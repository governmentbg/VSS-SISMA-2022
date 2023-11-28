using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SISMA.Core.Contracts;
using SISMA.Core.Extensions;
using SISMA.Core.Models.Reports;
using SISMA.Core.Models.StatReport;
using SISMA.Infrastructure.Data.Common;
using SISMA.Infrastructure.Data.Models.Nomenclatures;
using SISMA.Infrastructure.ViewModels.Common;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SISMA.Core.Services
{
    public class NomStatReportService : BaseService, INomStatReportService
    {
        private readonly INomenclatureService nomService;
        public NomStatReportService(
              IRepository _repo,
            ILogger<DataService> _logger,
            INomenclatureService _nomService)
        {
            repo = _repo;
            logger = _logger;
            nomService = _nomService;
        }

        #region  Управление на отчети
        public IQueryable<StatReportVM> StatReport_Select(FilterStatReportVM filter)
        {
            filter?.Sanitize();

            Expression<Func<NomStatReport, bool>> whereCategory = x => true;
            if (filter.ReportCategoryId > 0)
            {
                whereCategory = x => x.ReportCategoryId == filter.ReportCategoryId;
            }

            Expression<Func<NomStatReport, bool>> whereCatalog = x => true;
            if (filter.CatalogId > 0)
            {
                whereCatalog = x => x.CatalogId == filter.CatalogId;
            }
            Expression<Func<NomStatReport, bool>> whereReportType = x => true;
            if (filter.ReportTypeId > 0)
            {
                whereReportType = x => x.ReportTypeId == filter.ReportTypeId;
            }
            Expression<Func<NomStatReport, bool>> whereName = x => true;
            if (!string.IsNullOrEmpty(filter.Label))
            {
                whereName = x => EF.Functions.ILike(x.Label, filter.Label.ToPaternSearch());
            }
            return repo.AllReadonly<NomStatReport>()
                                .Where(x => x.Catalog.IntegrationId == filter.IntegrationId)
                                .Where(whereCategory)
                                .Where(whereCatalog)
                                .Where(whereReportType)
                                .Where(whereName)
                                .OrderBy(x => x.OrderNumber)
                                .Select(x => new StatReportVM
                                {
                                    Id = x.Id,
                                    Label = x.Label,
                                    CategoryName = x.ReportCategory.Label,
                                    CatalogName = x.Catalog.Label,
                                    IntegrationId = x.Catalog.IntegrationId,
                                    IntegrationName = x.Catalog.Integration.Label,
                                    ReportTypeName = x.ReportType.Label,
                                    DateStart = x.DateStart
                                }).AsQueryable();
        }

        public IQueryable<StatReportColVM> StatReportCol_Select(int statReportId)
        {
            return repo.AllReadonly<NomStatReportCol>()
                                .Where(x => x.StatReportId == statReportId)
                                .OrderBy(x => x.OrderNumber)
                                .Select(x => new StatReportColVM
                                {
                                    Id = x.Id,
                                    CatalogCodeId = x.CatalogCodeId,
                                    IbdCode = x.CatalogCode.Code,
                                    IbdName = x.CatalogCode.Label,
                                    Label = x.Label
                                }).AsQueryable();
        }

        public IQueryable<StatReportCaseCodeVM> StatReportCaseCode_Select(int statReportId)
        {
            return repo.AllReadonly<NomStatReportCode>()
                                .Where(x => x.StatReportId == statReportId)
                                .OrderBy(x => x.OrderNumber)
                                .Select(x => new StatReportCaseCodeVM
                                {
                                    Id = x.Id,
                                    Code = x.CaseCode.Code,
                                    CodeName = x.CaseCode.Label,
                                    Label = x.Label
                                }).AsQueryable();
        }


        public async Task<SaveResultVM> StatReport_SaveData(NomStatReport model)
        {
            if (repo.AllReadonly<NomStatReport>().Where(x => x.Label == model.Label
                        && x.ReportCategoryId == model.ReportCategoryId
                        && x.Id != model.Id).Any())
            {
                return new SaveResultVM(false, "Съществува отчет със същото наименование.");
            }

            try
            {
                if (model.SecondIntegrationId <= 0)
                {
                    model.SecondIntegrationId = null;
                    model.SecondCatalogId = null;
                    model.SecondCatalogCodeId = null;
                    model.RatioMultiplier = null;
                }

                if (model.Id > 0)
                {
                    var _saved = repo.GetById<NomStatReport>(model.Id);
                    repo.Detach(_saved);
                    model.OrderNumber = _saved.OrderNumber;
                    repo.Update(model);
                    await repo.SaveChangesAsync();
                    return new SaveResultVM(true);
                }
                else
                {
                    repo.Add(model);
                    await repo.SaveChangesAsync();
                    model.OrderNumber = model.Id;
                    await repo.SaveChangesAsync();
                    return new SaveResultVM(true);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"StatReport_SaveData - {model.Id}");
                return new SaveResultVM(false);
            }
        }

        public async Task<SaveResultVM> StatReportCol_SaveData(ColSelectVM model)
        {
            try
            {
                var savedCols = await repo.All<NomStatReportCol>()
                                .Where(x => x.StatReportId == model.StatReportId).ToListAsync();

                var newCols = model.SelectedList.Select(x => int.Parse(x)).ToArray();

                foreach (var item in savedCols)
                {
                    if (!newCols.Contains(item.CatalogCodeId))
                    {
                        repo.Delete(item);
                    }
                }

                foreach (var itemCol in newCols)
                {
                    if (!savedCols.Any(x => x.CatalogCodeId == itemCol))
                    {
                        var newItem = new NomStatReportCol()
                        {
                            StatReportId = model.StatReportId,
                            CatalogCodeId = itemCol,
                            Label = repo.GetPropById<NomCatalogCode, string>(x => x.Id == itemCol, x => x.Label),
                            OrderNumber = 0,
                            IsActive = true
                        };

                        await repo.AddAsync(newItem);
                        await repo.SaveChangesAsync();
                        newItem.OrderNumber = newItem.Id;
                        await repo.SaveChangesAsync();
                    }
                }

                return new SaveResultVM(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"StatReportCol_SaveData,ColSelectVM - {model.StatReportId}");
                return new SaveResultVM(false);
            }
        }

        public async Task<SaveResultVM> StatReportCol_SaveData(NomStatReportCol model)
        {
            try
            {
                if (repo.AllReadonly<NomStatReportCol>()
                                .Where(x => x.StatReportId == model.StatReportId
                                && x.CatalogCodeId == model.CatalogCodeId
                                && x.Id != model.Id).Any())
                {
                    return new SaveResultVM(false, "Избрания ИБД код вече е добавен към отчета.");
                }

                if (model.Id > 0)
                {
                    var _saved = repo.GetById<NomStatReportCol>(model.Id);
                    repo.Detach(_saved);
                    model.OrderNumber = _saved.OrderNumber;
                    repo.Update(model);
                    await repo.SaveChangesAsync();
                    return new SaveResultVM(true);
                }
                else
                {
                    repo.Add(model);
                    await repo.SaveChangesAsync();
                    model.OrderNumber = model.Id;
                    await repo.SaveChangesAsync();
                    return new SaveResultVM(true);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"StatReportCol_SaveData - {model.Id}");
                return new SaveResultVM(false);
            }
        }

        public async Task<SaveResultVM> StatReportCaseCode_SaveData(NomStatReportCode model)
        {
            try
            {

                if (model.Id > 0)
                {
                    var _saved = repo.GetById<NomStatReportCode>(model.Id);
                    repo.Detach(_saved);
                    model.OrderNumber = _saved.OrderNumber;
                    repo.Update(model);
                    await repo.SaveChangesAsync();
                    return new SaveResultVM(true);
                }
                else
                {
                    repo.Add(model);
                    await repo.SaveChangesAsync();
                    model.OrderNumber = model.Id;
                    await repo.SaveChangesAsync();
                    return new SaveResultVM(true);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"StatReportCaseCode_SaveData - {model.Id}");
                return new SaveResultVM(false);
            }
        }

        #endregion

        public IQueryable<StatReportCategoryVM> StatReportCategory_Select(FilterStatReportCategoryVM filter)
        {
            filter.Sanitize();
            return repo.AllReadonly<NomStatReportCategory>()
                        .Where(x => x.IntegrationId == (filter.IntegrationId ?? x.IntegrationId))
                        .OrderBy(x => x.IntegrationId).ThenBy(x => x.OrderNumber)
                        .Select(x => new StatReportCategoryVM
                        {
                            Id = x.Id,
                            IntegrationName = x.Integration.Label,
                            Label = x.Label,
                            DateStart = x.DateStart,
                            IsActive = x.IsActive
                        }).AsQueryable();
        }

        public async Task<SaveResultVM> StatReportCategory_SaveData(NomStatReportCategory model)
        {
            try
            {

                if (model.Id > 0)
                {
                    var _saved = repo.GetById<NomStatReportCategory>(model.Id);
                    repo.Detach(_saved);
                    model.OrderNumber = _saved.OrderNumber;
                    repo.Update(model);
                    await repo.SaveChangesAsync();
                    return new SaveResultVM(true);
                }
                else
                {
                    repo.Add(model);
                    await repo.SaveChangesAsync();
                    model.OrderNumber = model.Id;
                    await repo.SaveChangesAsync();
                    return new SaveResultVM(true);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"StatReportCategory_SaveData - {model.Id}");
                return new SaveResultVM(false);
            }
        }

    }
}


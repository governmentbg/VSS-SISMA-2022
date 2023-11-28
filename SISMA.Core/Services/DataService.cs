using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SISMA.Core.Contracts;
using SISMA.Core.Extensions;
using SISMA.Core.Models;
using SISMA.Core.Models.Common;
using SISMA.Core.Models.Reports;
using SISMA.Infrastructure.Constants;
using SISMA.Infrastructure.Contracts.Data;
using SISMA.Infrastructure.Data.Common;
using SISMA.Infrastructure.Data.Models.Common;
using SISMA.Infrastructure.Data.Models.Ekatte;
using SISMA.Infrastructure.Data.Models.Nomenclatures;
using SISMA.Infrastructure.Data.Models.Report;
using SISMA.Infrastructure.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SISMA.Core.Services
{
    public class DataService : BaseService, IDataService
    {
        private readonly INomenclatureService nomService;
        public DataService(
              IRepository _repo,
            ILogger<DataService> _logger,
            INomenclatureService _nomService)
        {
            repo = _repo;
            logger = _logger;
            nomService = _nomService;
        }

        public IQueryable<ReportDataVM> Select(FilterReportData filter)
        {
            filter.Sanitize();

            Expression<Func<ReportData, bool>> whereId = x => true;
            if (filter.Id > 0)
            {
                whereId = x => x.Id == filter.Id;
            }
            Expression<Func<ReportData, bool>> whereIntegration = x => true;
            if (filter.IntegrationId > 0)
            {
                whereIntegration = x => x.IntegrationId == filter.IntegrationId;
            }

            Expression<Func<ReportData, bool>> whereCatalog = x => true;
            if (filter.CatalogId > 0)
            {
                whereCatalog = x => x.CatalogId == filter.CatalogId;
            }
            Expression<Func<ReportData, bool>> whereDateFrom = x => true;
            if (filter.DateFrom != null)
            {
                whereDateFrom = x => x.ReportDate >= filter.DateFrom.Value;
            }
            Expression<Func<ReportData, bool>> whereDateTo = x => true;
            if (filter.DateTo != null)
            {
                whereDateTo = x => x.ReportDate <= filter.DateTo.MakeEndDate();
            }
            Expression<Func<ReportData, bool>> wherePeriodNo = x => true;
            if (filter.PeriodNo > 0)
            {
                wherePeriodNo = x => x.ReportPeriod == filter.PeriodNo;
            }
            Expression<Func<ReportData, bool>> wherePeriodYear = x => true;
            if (filter.PeriodYear > 0)
            {
                wherePeriodYear = x => x.ReportYear == filter.PeriodYear;
            }
            Expression<Func<ReportData, bool>> whereReportState = x => true;
            if (filter.ReportStateId > 0)
            {
                whereReportState = x => x.ReportStateId == filter.ReportStateId;
            }

            return repo.AllReadonly<ReportData>()
                            .Where(whereId)
                            .Where(whereIntegration)
                            .Where(whereCatalog)
                            .Where(whereDateFrom)
                            .Where(whereDateTo)
                            .Where(wherePeriodNo)
                            .Where(wherePeriodYear)
                            .Where(whereReportState)
                            .Select(x => new ReportDataVM
                            {
                                Id = x.Id,
                                IntegrationId = x.IntegrationId,
                                IntegrationName = x.Integration.Label,
                                CatalogName = x.Catalog.Label,
                                ReportSourceName = x.ReportSource.Label,
                                ReportDate = x.ReportDate,
                                PeriodNo = x.ReportPeriod,
                                PeriodYear = x.ReportYear,
                                StatusName = x.ReportState.Label,
                                ReportStateId = x.ReportStateId
                            });
        }

        public SaveResultVM Manage(ReportDataVM model)
        {
            var saved = repo.GetById<ReportData>(model.Id);
            if (saved != null)
            {
                saved.ReportStateId = model.ReportStateId;
                repo.SaveChanges();
                return new SaveResultVM(true);
            }
            return new SaveResultVM(false);
        }


        public IQueryable<EkatteItemVM> Distance_Select(FilterEkatteItemVM filter)
        {
            //Expression<Func<EkEkatte, bool>> whereMunicipality = x => true;
            //if (filter.MunicipalityId > 0)
            //{
            //    whereMunicipality = x => x.MunicipalId == filter.MunicipalityId;
            //}
            //Expression<Func<EkEkatte, bool>> whereCityName = x => true;
            //if (!string.IsNullOrEmpty(filter.CityName))
            //{
            //    whereCityName = x => EF.Functions.ILike(x.Name, filter.CityName.ToPaternSearch());
            //}
            int[] courtTypes = filter.CourtTypes.ToIntArray();
            int? distanceType = null;
            if (filter.DistanceType == 1)
            {
                distanceType = filter.DistanceType;
            }
            return repo.AllReadonly<EkEkatte>()
                                    //.Where(whereMunicipality)
                                    .Where(x => x.Ekatte == filter.EkatteCode)
                                    .Select(x => new EkatteItemVM
                                    {
                                        Id = x.Id,
                                        CityName = $"{x.TVM}{x.Name}",
                                        MunicipalityName = $"общ.{x.Munincipality.Name}",
                                        Latitude = x.Lat,
                                        Longitute = x.Lon,
                                        Distances = x.Distances.Where(d => courtTypes.Contains(d.Court.CourtTypeId))
                                                            .Where(d => d.DistanceType == (distanceType ?? d.DistanceType))
                                                            .OrderByDescending(d => d.Distance)
                                                            .Select(d => new DistanceItemVM
                                                            {
                                                                Id = d.CourtId,
                                                                CourtName = d.Court.Label,
                                                                Distance = d.Distance,
                                                                Duration = d.Duration,
                                                                Longitute = d.Court.Longitude,
                                                                Latitude = d.Court.Latitude
                                                            }).ToList()

                                    }).AsQueryable();
        }

        public async Task<SaveResultVM> SaveData(SismaModel model)
        {
            try
            {
                var entity = await mapModel(model);
                //return new SaveResultVM(true);
                switch (model.Context.MethodName)
                {
                    case SismaConstants.Methods.Add:
                        {
                            var savedReport = await getReportDataByPeriod(entity);
                            if (savedReport != null)
                            {
                                entity.ReportStateId = NomenclatureConstants.ReportStates.New;
                            }

                            await repo.AddAsync(entity);
                        }
                        break;
                    //case SismaConstants.Methods.Edit:
                    //    {
                    //        var savedReport = await getReportDataByPeriod(entity);

                    //        if (savedReport == null)
                    //        {
                    //            throw new SismaMappingException(SismaConstants.ErrorCodes.InvalidReport,
                    //                $"Ненамерен отчет за период {entity.ReportPeriod}/{entity.ReportYear}"
                    //                );
                    //        }

                    //        savedReport.ReportStateId = NomenclatureConstants.ReportStates.Updated;
                    //        repo.Add(entity);
                    //        break;
                    //    }
                    case SismaConstants.Methods.Delete:
                        {
                            var savedReport = await getReportDataByPeriod(entity);

                            if (savedReport == null)
                            {
                                throw new SismaMappingException(SismaConstants.ErrorCodes.InvalidReport,
                                    $"Ненамерен отчет за период {entity.ReportPeriod}/{entity.ReportYear}"
                                    );
                            }

                            savedReport.ReportStateId = NomenclatureConstants.ReportStates.Deleted;
                            entity.ReportStateId = NomenclatureConstants.ReportStates.Deleted;
                            repo.Add(entity);
                            break;
                        }
                    default:
                        throw new SismaMappingException(SismaConstants.ErrorCodes.InvalidValue,
                            $"Невалидна стойност за метод {model.Context.MethodName} (Context.MethodName)"
                        );
                }
                await repo.SaveChangesAsync();
                return new SaveResultVM(true);
            }
            catch (SismaMappingException mex)
            {
                logger.LogError($"MappingError: {0}, {1}", mex.ErrorCode, mex.ErrorMessage);
                return new SaveResultVM()
                {
                    IsSuccessfull = false,
                    ErrorCode = mex.ErrorCode.ToString(),
                    ErrorMessage = mex.ErrorMessage
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return new SaveResultVM()
                {
                    IsSuccessfull = false,
                    ErrorCode = SismaConstants.ErrorCodes.GeneralException,
                    ErrorMessage = ex.Message
                };
            }
        }

        private async Task<ReportData> getReportDataByPeriod(ReportData entity)
        {
            var savedReport = await repo.All<ReportData>()
                                                         .Where(x => x.IntegrationId == entity.IntegrationId)
                                                         .Where(x => x.CatalogId == entity.CatalogId)
                                                         .Where(x => x.ReportPeriod == entity.ReportPeriod)
                                                         .Where(x => x.ReportYear == entity.ReportYear)
                                                         .Where(x => x.ReportStateId == NomenclatureConstants.ReportStates.Saved)
                                                         .FirstOrDefaultAsync();


            return savedReport;
        }

        /// <summary>
        /// Маппинг на данни от трансферен протокол към моделите на СИСМА
        /// </summary>
        /// <param name="model">Входни данни</param>
        /// <returns></returns>
        private async Task<ReportData> mapModel(SismaModel model)
        {
            var reportData = new ReportData();
            reportData.IntegrationId = await nomService.GetNomIdByCode<NomIntegration>(model.Context.IntegrationType);
            if (reportData.IntegrationId == 0)
            {
                throw new SismaMappingException(SismaConstants.ErrorCodes.InvalidCode, $"Невалиден код за интеграция {model.Context.IntegrationType} (Context.IntegrationType)");
            }
            reportData.CatalogId = await nomService.GetNomIdByCode<NomCatalog>(model.Context.ReportType);
            if (reportData.CatalogId == 0)
            {
                throw new SismaMappingException(SismaConstants.ErrorCodes.InvalidCode, $"Невалиден код на отчет {model.Context.ReportType} (Context.ReportType)");
            }
            reportData.ReportPeriod = model.Context.PeriodNumber;
            reportData.ReportYear = model.Context.PeriodYear;
            reportData.ReportStateId = NomenclatureConstants.ReportStates.Saved;
            reportData.ReportSourceId = NomenclatureConstants.ReportSources.API;
            reportData.ReportDate = DateTime.Now;
            if (model.Context.FromFTP)
            {
                reportData.ReportSourceId = NomenclatureConstants.ReportSources.FTP;
            }

            if (model.Context.MethodName == SismaConstants.Methods.Delete)
            {
                return reportData;
            }

            switch (reportData.IntegrationId)
            {
                case NomenclatureConstants.Integrations.EISS:
                    await mapEiss(model, reportData);
                    break;
                case NomenclatureConstants.Integrations.UIS:
                    await mapUis(model, reportData);
                    break;
                case NomenclatureConstants.Integrations.NSI:
                    await mapNsi(model, reportData);
                    break;
                case NomenclatureConstants.Integrations.EISPP:
                    await mapEisppMunicipality(model, reportData);
                    break;
                default:
                    break;
            }
            return reportData;
        }

        /// <summary>
        /// Маппинг на данни за ЕИСС, по съдилища/шифри/съдии
        /// </summary>
        /// <param name="model">Входни данни</param>
        /// <param name="target">Отчет за запис</param>
        /// <returns></returns>
        private async Task<bool> mapEiss(SismaModel model, ReportData target)
        {
            if (target.ReportPeriod < 1 && target.ReportPeriod > 12)
            {
                throw new SismaMappingException(SismaConstants.ErrorCodes.InvalidValue, $"Невалидна стойност за период {target.ReportPeriod} (Context.PeriodNumber)");
            }

            var result = new List<ReportEiss>();
            var _catalog = nomService.GetItem<NomCatalog>(target.CatalogId);
            var courts = await nomService.GetCommonObjectItemList<CommonCourt>();
            var catalogCodes = await nomService.GetCommonItemList<NomCatalogCode>(x => x.CatalogId == target.CatalogId);
            var caseCodes = await nomService.GetCommonItemList<NomCaseCode>();
            var subjects = getSubjectsFromModel(model, _catalog.DetailType);
            await appendUpdateSubjects(NomenclatureConstants.SubjectTypes.Judge, subjects);
            foreach (var code in model.Codes)
            {
                var entityCodes = code.Details
                                      .GroupBy(x => x.EntityCode)
                                      .Select(x => x.Key).ToArray();

                foreach (var entityCode in entityCodes)
                {
                    var newCourtItem = new ReportEiss();
                    newCourtItem.CatalogCodeId = catalogCodes.GetIdByCode(code.IbdCode);
                    if (newCourtItem.CatalogCodeId == 0)
                    {
                        throw new SismaMappingException(SismaConstants.ErrorCodes.InvalidCode, $"Невалиден ИБД код {code.IbdCode}");
                    }
                    newCourtItem.CourtId = courts.GetIdByCode(entityCode);
                    if (newCourtItem.CourtId == 0)
                    {
                        throw new SismaMappingException(SismaConstants.ErrorCodes.InvalidCode, $"Невалиден код на съд {entityCode}");
                    }
                    newCourtItem.CourtObjectId = courts.GetObjectIdByCode(entityCode);
                    var courtCount = code.Count;
                    if (courtCount == 0)
                    {
                        var detailsCount = code.Details.Where(d => d.EntityCode == entityCode).Select(d => d.Count).Sum();
                        if (detailsCount > 0)
                        {
                            courtCount = detailsCount;
                        }
                    }
                    newCourtItem.CountValue = courtCount;
                    result.Add(newCourtItem);
                    List<ReportEissCode> courtCodes = new List<ReportEissCode>();
                    List<ReportEissCourt> eissCourts = new List<ReportEissCourt>();
                    List<ReportEissSubject> courtSubjects = new List<ReportEissSubject>();

                    foreach (var detail in code.Details.Where(x => x.EntityCode == entityCode))
                    {
                        switch (_catalog.DetailType)
                        {
                            case NomenclatureConstants.CatalogDetailsType.CaseCode:
                                var newCaseCode = new ReportEissCode();
                                newCaseCode.CaseCodeId = caseCodes.GetIdByCode(detail.SubjectCode);
                                if (newCaseCode.CaseCodeId == 0)
                                {
                                    throw new SismaMappingException(SismaConstants.ErrorCodes.InvalidCode, $"Невалиден код на статистически шифър {detail.SubjectCode}");
                                }
                                newCaseCode.CountValue = detail.Count;
                                courtCodes.Add(newCaseCode);
                                break;
                            case NomenclatureConstants.CatalogDetailsType.Court:
                                var newCourt = new ReportEissCourt();
                                newCourt.CourtId = courts.GetIdByCode(detail.SubjectCode);
                                if (newCourt.CourtId == 0)
                                {
                                    // throw new SismaMappingException(SismaConstants.ErrorCodes.InvalidCode, $"Невалиден код на съд {detail.SubjectCode}");
                                }
                                else
                                {
                                    newCourt.CountValue = detail.Count;
                                    eissCourts.Add(newCourt);
                                }
                                break;
                            case NomenclatureConstants.CatalogDetailsType.Subject:
                                var newSubject = new ReportEissSubject();
                                newSubject.SubjectId = subjects.GetIdByCode(detail.SubjectCode);
                                newSubject.CountValue = detail.Count;
                                courtSubjects.Add(newSubject);
                                break;
                        }
                    }
                    if (courtCodes.Count > 0)
                    {
                        newCourtItem.Codes = courtCodes;
                    }
                    if (eissCourts.Count > 0)
                    {
                        newCourtItem.Courts = eissCourts;
                    }
                    if (courtSubjects.Count > 0)
                    {
                        newCourtItem.Subjects = courtSubjects;
                    }
                }
            }

            if (result.Any())
            {
                target.DetailsEISS = result;
            }

            return true;
        }

        /// <summary>
        /// Маппинг на данни за НСИ, по съдилища/шифри/съдии
        /// </summary>
        /// <param name="model">Входни данни</param>
        /// <param name="target">Отчет за запис</param>
        /// <returns></returns>
        private async Task<bool> mapNsi(SismaModel model, ReportData target)
        {
            if (target.ReportPeriod < 1 && target.ReportPeriod > 12)
            {
                throw new SismaMappingException(SismaConstants.ErrorCodes.InvalidValue, $"Невалидна стойност за период {target.ReportPeriod} (Context.PeriodNumber)");
            }

            var result = new List<ReportNsi>();
            var _catalog = nomService.GetItem<NomCatalog>(target.CatalogId);
            var ekattes = await repo.AllReadonly<EkEkatte>()
                                        .Select(x => new CommonItemVM
                                        {
                                            Id = x.Id,
                                            Code = x.Ekatte,
                                            Label = x.Name
                                        }).ToListAsync();

            var catalogCodes = await nomService.GetCommonItemList<NomCatalogCode>(x => x.CatalogId == target.CatalogId);
            foreach (var code in model.Codes)
            {
                foreach (var entity in code.Details)
                {
                    var newCourtItem = new ReportNsi();
                    newCourtItem.CatalogCodeId = catalogCodes.GetIdByCode(code.IbdCode);
                    if (newCourtItem.CatalogCodeId == 0)
                    {
                        throw new SismaMappingException(SismaConstants.ErrorCodes.InvalidCode, $"Невалиден ИБД код {code.IbdCode}");
                    }
                    newCourtItem.EkatteId = ekattes.GetIdByCode(entity.EntityCode);
                    if (newCourtItem.EkatteId == 0)
                    {
                        throw new SismaMappingException(SismaConstants.ErrorCodes.InvalidCode, $"Невалиден код на населено място {entity.EntityCode}");
                    }
                    newCourtItem.CountValue = entity.Count;
                    //if (entity.Amount.HasValue)
                    //{
                    //    newCourtItem.AmmountValue = entity.Amount.Value;
                    //}
                    newCourtItem.AmmountValue = entity.Amount;
                    result.Add(newCourtItem);
                }
            }

            if (result.Any())
            {
                target.DetailsNSI = result;
            }

            return true;
        }

        /// <summary>
        /// Маппинг на данни за ЕИСПП, по общини
        /// </summary>
        /// <param name="model">Входни данни</param>
        /// <param name="target">Отчет за запис</param>
        /// <returns></returns>
        private async Task<bool> mapEisppMunicipality(SismaModel model, ReportData target)
        {
            if (target.ReportPeriod < 1 && target.ReportPeriod > 12)
            {
                throw new SismaMappingException(SismaConstants.ErrorCodes.InvalidValue, $"Невалидна стойност за период {target.ReportPeriod} (Context.PeriodNumber)");
            }

            var result = new List<ReportEisppMunicipality>();
            var _catalog = nomService.GetItem<NomCatalog>(target.CatalogId);

            var municipalities = await repo.AllReadonly<EkMunincipality>()
                                        .Select(x =>
                                        new CommonItemVM
                                        {
                                            Id = x.MunicipalityId,
                                            Code = x.Municipality,
                                            Label = x.Name
                                        }).ToListAsync();

            var catalogCodes = await nomService.GetCommonItemList<NomCatalogCode>(x => x.CatalogId == target.CatalogId);
            foreach (var code in model.Codes)
            {
                foreach (var entity in code.Details)
                {
                    var newItem = new ReportEisppMunicipality();
                    newItem.CatalogCodeId = catalogCodes.GetIdByCode(code.IbdCode);
                    if (newItem.CatalogCodeId == 0)
                    {
                        throw new SismaMappingException(SismaConstants.ErrorCodes.InvalidCode, $"Невалиден ИБД код {code.IbdCode}");
                    }
                    newItem.MunicipalityId = municipalities.GetIdByCode(entity.EntityCode);
                    if (newItem.MunicipalityId == 0)
                    {
                        throw new SismaMappingException(SismaConstants.ErrorCodes.InvalidCode, $"Невалиден код на община {entity.EntityCode}");
                    }
                    newItem.CountValue = entity.Count;
                    result.Add(newItem);
                }
            }

            if (result.Any())
            {
                target.DetailsEisppMunicipality = result;
            }

            return true;
        }

        /// <summary>
        /// Маппинг на данни за УИС, по съдилища/шифри/съдии
        /// </summary>
        /// <param name="model">Входни данни</param>
        /// <param name="target">Отчет за запис</param>
        /// <returns></returns>
        private async Task<bool> mapUis(SismaModel model, ReportData target)
        {
            if (target.ReportPeriod < 1 && target.ReportPeriod > 12)
            {
                throw new SismaMappingException(SismaConstants.ErrorCodes.InvalidValue, $"Невалидна стойност за период {target.ReportPeriod} (Context.PeriodNumber)");
            }

            var result = new List<ReportUis>();
            var _catalog = nomService.GetItem<NomCatalog>(target.CatalogId);
            var institutions = await nomService.GetCommonObjectItemList<CommonProsecutor>();

            var catalogCodes = await nomService.GetCommonItemList<NomCatalogCode>(x => x.CatalogId == target.CatalogId);
            foreach (var code in model.Codes)
            {
                foreach (var entity in code.Details)
                {
                    var newCourtItem = new ReportUis();
                    newCourtItem.CatalogCodeId = catalogCodes.GetIdByCode(code.IbdCode);
                    if (newCourtItem.CatalogCodeId == 0)
                    {
                        throw new SismaMappingException(SismaConstants.ErrorCodes.InvalidCode, $"Невалиден ИБД код {code.IbdCode}");
                    }
                    newCourtItem.ProsecutorId = institutions.GetIdByCode(entity.EntityCode);
                    if (newCourtItem.ProsecutorId == 0)
                    {
                        throw new SismaMappingException(SismaConstants.ErrorCodes.InvalidCode, $"Невалиден код на прокуратура {entity.EntityCode}");
                    }
                    newCourtItem.CountValue = entity.Count;

                    result.Add(newCourtItem);
                }
            }

            if (result.Any())
            {
                target.DetailsUIS = result;
            }

            return true;
        }

        private List<CommonItemVM> getSubjectsFromModel(SismaModel model, int? detailsType)
        {
            if (detailsType != NomenclatureConstants.CatalogDetailsType.Subject)
            {
                return null;
            }
            return model.Codes.SelectMany(x => x.Details)
                            .GroupBy(x => new { x.SubjectCode, x.SubjectName })
                            .Select(x => new CommonItemVM
                            {
                                Code = x.Key.SubjectCode,
                                Label = x.Key.SubjectName
                            }).ToList();
        }

        private async Task appendUpdateSubjects(int subjectTypeId, List<CommonItemVM> subjects)
        {
            if (subjects == null)
            {
                return;
            }
            //Всички ЕГН-та на лица
            var subjectCodes = subjects.Select(x => x.Code).ToArray();
            var forSaveChanges = false;

            //Всички запазени лица по подадените ЕГН-та
            var saved = await repo.All<CommonSubject>()
                                .Where(x => x.SubjectTypeId == subjectTypeId)
                                .Where(x => subjectCodes.Contains(x.Code))
                                .Where(x => x.IsActive)
                                .ToListAsync();
            //Всички намерени лица се редактират новоподадените данни
            foreach (var savedItem in saved)
            {
                var forUpdate = subjects.Where(x => x.Code == savedItem.Code).FirstOrDefault();
                //Само ако се различават имената
                if (savedItem.Label != forUpdate.Label)
                {
                    savedItem.Label = forUpdate.Label;
                    forSaveChanges = true;
                }
                forUpdate.Id = savedItem.Id;
            }

            var appendList = new List<CommonSubject>();
            foreach (var forAppend in subjects)
            {
                //Вече намерените се пропускат
                if (forAppend.Id > 0)
                {
                    continue;
                }

                appendList.Add(
                    new CommonSubject()
                    {
                        SubjectTypeId = subjectTypeId,
                        Code = forAppend.Code,
                        Label = forAppend.Label,
                        IsActive = true,
                        DateStart = DateTime.Now
                    }
               );
            }

            repo.AddRange(appendList);

            if (forSaveChanges || appendList.Count > 0)
            {
                //Ако има за редакция или има нови - промените се записват
                await repo.SaveChangesAsync();
            }
            if (appendList.Count > 0)
            {
                foreach (var sbj in subjects)
                {
                    if (sbj.Id > 0)
                    {
                        continue;
                    }
                    //Вземат се Id-тата на всички нови лица
                    sbj.Id = appendList.Where(x => x.Code == sbj.Code).Select(x => x.Id).FirstOrDefault();
                }
            }
        }

        public async Task<SismaModel> TestModel()
        {
            SismaModel model = new SismaModel()
            {
                Context = new SismaContextModel()
                {
                    IntegrationType = "1",
                    PeriodNumber = 13,
                    PeriodYear = 2021,
                    ReportType = "1-01",
                    MethodName = SismaConstants.Methods.Add
                }
            };

            var codes = await repo.AllReadonly<NomCatalogCode>()
                                    .Where(x => x.CatalogId == 1)
                                    .OrderBy(x => x.Code)
                                    .Select(x => x.Code).ToListAsync();

            var courts = await repo.AllReadonly<CommonCourt>()
                                    .OrderBy(x => x.Code)
                                    .Select(x => x.Code).ToListAsync();

            var caseCodes = await repo.AllReadonly<NomCaseCode>()
                                    .Where(x => x.Id < 200)
                                    .OrderBy(x => x.Code)
                                    .Select(x => x.Code).ToListAsync();

            var _codes = new List<SismaCodeModel>();
            foreach (var code in codes)
            {
                var newCode = new SismaCodeModel()
                {
                    IbdCode = code,
                    Count = rndCount()
                };

                var _details = new List<SismaCodeDetailModel>();
                foreach (var court in courts)
                {
                    foreach (var caseCode in caseCodes)
                    {
                        _details.Add(new SismaCodeDetailModel()
                        {
                            EntityCode = court,
                            SubjectCode = caseCode,
                            Count = rndCount()
                        });
                    }
                }
                newCode.Details = _details.ToArray();

                _codes.Add(newCode);
            }
            model.Codes = _codes.ToArray();
            return model;
        }

        public async Task<SismaModel> TestModelCourts()
        {
            SismaModel model = new SismaModel()
            {
                Context = new SismaContextModel()
                {
                    IntegrationType = "1",
                    PeriodNumber = 12,
                    PeriodYear = 2021,
                    ReportType = "1-02",
                    MethodName = SismaConstants.Methods.Add
                }
            };

            var codes = await repo.AllReadonly<NomCatalogCode>()
                                    .Where(x => x.CatalogId == 2)
                                    .OrderBy(x => x.Code)
                                    .Select(x => x.Code).ToListAsync();

            var courts = await repo.AllReadonly<CommonCourt>()
                                    .OrderBy(x => x.Code)
                                    .Select(x => x.Code).ToListAsync();

            var otherCourts = await repo.AllReadonly<CommonCourt>()
                                    .Where(x => x.Id < 20)
                                   .OrderBy(x => x.Code)
                                   .Select(x => x.Code).ToListAsync();

            var _codes = new List<SismaCodeModel>();
            foreach (var code in codes)
            {
                var newCode = new SismaCodeModel()
                {
                    IbdCode = code,
                    Count = rndCount()
                };

                var _details = new List<SismaCodeDetailModel>();
                foreach (var court in courts)
                {
                    foreach (var subjectCode in otherCourts)
                    {
                        _details.Add(new SismaCodeDetailModel()
                        {
                            EntityCode = court,
                            SubjectCode = subjectCode,
                            Count = rndCount()
                        });
                    }
                }
                newCode.Details = _details.ToArray();

                _codes.Add(newCode);
            }
            model.Codes = _codes.ToArray();
            return model;
        }

        public async Task<SismaModel> TestModelTotal(int catId, int year)
        {
            SismaModel model = new SismaModel()
            {
                Context = new SismaContextModel()
                {
                    IntegrationType = "1",
                    PeriodNumber = 12,
                    PeriodYear = year,
                    ReportType = $"1-{catId:D2}",
                    MethodName = SismaConstants.Methods.Add
                }
            };

            var codes = await repo.AllReadonly<NomCatalogCode>()
                                    .Where(x => x.CatalogId == catId)
                                    .OrderBy(x => x.Code)
                                    .Select(x => x.Code).ToListAsync();

            var courts = await repo.AllReadonly<CommonCourt>()
                                    .OrderBy(x => x.Code)
                                    .Select(x => x.Code).ToListAsync();

            var _codes = new List<SismaCodeModel>();
            foreach (var code in codes)
            {
                var newCode = new SismaCodeModel()
                {
                    IbdCode = code,
                    Count = rndCount()
                };

                var _details = new List<SismaCodeDetailModel>();
                foreach (var court in courts)
                {
                    _details.Add(new SismaCodeDetailModel()
                    {
                        EntityCode = court,
                        Count = rndCount()
                    });
                }
                newCode.Details = _details.ToArray();

                _codes.Add(newCode);
            }
            model.Codes = _codes.ToArray();
            return model;
        }


        public async Task<SismaModel> TestModelNSI()
        {
            SismaModel model = new SismaModel()
            {
                Context = new SismaContextModel()
                {
                    IntegrationType = "6",
                    PeriodNumber = 12,
                    PeriodYear = 2021,
                    ReportType = "6-01",
                    MethodName = SismaConstants.Methods.Add
                }
            };

            var codes = await repo.AllReadonly<NomCatalogCode>()
                                    .Where(x => x.Id == 1304)
                                    .OrderBy(x => x.Code)
                                    .Select(x => x.Code).ToListAsync();

            var ekatte = await repo.AllReadonly<EkEkatte>()
                                    .Select(x => new
                                    {
                                        x.Ekatte,
                                        x.Category
                                    }).ToListAsync();


            var _codes = new List<SismaCodeModel>();
            foreach (var code in codes)
            {
                var newCode = new SismaCodeModel()
                {
                    IbdCode = code,
                    Count = rndCount()
                };

                var _details = new List<SismaCodeDetailModel>();
                foreach (var city in ekatte)
                {

                    int count = rndCount(1000);

                    switch (city.Category)
                    {
                        case "1":
                            count = rndCount(500000);
                            break;
                        case "2":
                            count = rndCount(100000);
                            break;
                        case "3":
                            count = rndCount(50000);
                            break;
                        case "4":
                            count = rndCount(40000);
                            break;
                        case "5":
                            count = rndCount(20000);
                            break;
                        case "6":
                            count = rndCount(5000);
                            break;
                        case "7":
                            count = rndCount(2000);
                            break;
                        case "8":
                            count = rndCount(500);
                            break;
                        default:
                            break;
                    }

                    _details.Add(new SismaCodeDetailModel()
                    {
                        EntityCode = city.Ekatte,
                        Count = count
                    });
                }
                newCode.Details = _details.ToArray();

                _codes.Add(newCode);
            }
            model.Codes = _codes.ToArray();
            return model;
        }

        private int rndCount(int maxCount = 800)
        {
            Random rnd = new Random();
            return rnd.Next(maxCount);
        }
    }
}

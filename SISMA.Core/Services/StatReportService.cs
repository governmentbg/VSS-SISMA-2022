using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SISMA.Core.Contracts;
using SISMA.Core.Extensions;
using SISMA.Core.Models.StatReport;
using SISMA.Infrastructure.Constants;
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
    public class StatReportService : BaseService, IStatReportService
    {
        private readonly INomenclatureService nomService;
        public StatReportService(
              IRepository _repo,
            ILogger<DataService> _logger,
            INomenclatureService _nomService)
        {
            repo = _repo;
            logger = _logger;
            nomService = _nomService;
        }

        public async Task<DataTablesStatReportVM> Get_ReportData(int statReportId, int[] entityList, int periodNo, int periodYear)
        {

            StatReportGetDataVM reportInfo = await repo.AllReadonly<NomStatReport>()
                                       .Include(x => x.Codes)
                                       .Include(x => x.Columns)
                                       .Include(x => x.ReportType)
                                       .Include(x => x.Catalog)
                                       .Where(x => x.Id == statReportId)
                                       .Select(x => new StatReportGetDataVM
                                       {
                                           Id = x.Id,
                                           IntegrationId = x.Catalog.IntegrationId,
                                           CatalogId = x.CatalogId,
                                           Label = x.Label,
                                           Description = x.Description,
                                           ReportTypeId = x.ReportTypeId,
                                           RowLabel = x.RowLabel,
                                           DefaultRowLabel = x.ReportType.DefaultRowLabel,
                                           ReportEntityList = x.EntityList,
                                           FilterEntityList = entityList,
                                           PeriodNo = periodNo,
                                           PeriodYear = periodYear,
                                           SecondIntegrationId = x.SecondIntegrationId,
                                           SecondCatalogId = x.SecondCatalogId,
                                           SecondCatalogCodeId = x.SecondCatalogCodeId,
                                           RatioMultiplier = x.RatioMultiplier,
                                           Columns = x.Columns.Where(c => c.IsActive).OrderBy(c => c.OrderNumber).Select(c => new StatReportColVM
                                           {
                                               Id = c.Id,
                                               CatalogCodeId = c.CatalogCodeId,
                                               Label = c.Label
                                           }).ToList()
                                       })
                                       .FirstOrDefaultAsync();

            if (reportInfo == null)
                return null;

            try
            {
                switch (reportInfo.ReportTypeId)
                {
                    case NomenclatureConstants.StatReportType.ByIBD:
                        return await reportDataByCatalogCode(reportInfo);
                    case NomenclatureConstants.StatReportType.ByIBD3Years:
                        return await reportData3yearsByCatalogCode(reportInfo, false);
                    case NomenclatureConstants.StatReportType.ByIBD3YearsInverse:
                        return await reportData3yearsByCatalogCode(reportInfo, true);
                    case NomenclatureConstants.StatReportType.ByIBD3YearsMixed:
                        return await reportData3yearsByCatalogCodeMixed(reportInfo);

                    case NomenclatureConstants.StatReportType.ByIbdByOSV:
                        return await reportDataByIbdByOSV(reportInfo);


                    case NomenclatureConstants.StatReportType.ByCaseCode:
                        return await eiss_reportDataByCaseCode(reportInfo);
                    case NomenclatureConstants.StatReportType.ByCourt:
                        return await eiss_reportDataByCourt(reportInfo);
                    default:
                        return null;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private DataTablesStatReportVM initDataTablesModelFromReport(StatReportGetDataVM reportInfo, dynamic[] data, string periodInfo = "")
        {
            var rowLabel = reportInfo.RowLabel;
            if (string.IsNullOrEmpty(rowLabel))
            {
                rowLabel = reportInfo.DefaultRowLabel;
            }
            return new DataTablesStatReportVM()
            {
                ReportTitle = reportInfo.Label,
                ReportSubtitle = $"{reportInfo.Description} {periodInfo}",
                RowLabel = rowLabel,
                GroupType = reportInfo.ReportTypeId,
                Data = data,
                Columns = new List<DataTablesStatReportColumnsVM>() {
                    new DataTablesStatReportColumnsVM() {
                        ColumnName ="order",
                        Label ="No",
                        DataCol=false
                        },
                new DataTablesStatReportColumnsVM() {
                        ColumnName ="label",
                        Label =rowLabel,
                        DataCol=false
                        }}
            };
        }

        private async Task<DataTablesStatReportVM> eiss_reportDataByCaseCode(StatReportGetDataVM reportInfo)
        {

            var reportCaseCodes = repo.AllReadonly<NomStatReportCode>()
                                        .Where(x => x.StatReportId == reportInfo.Id && x.IsActive)
                                        .OrderBy(x => x.OrderNumber)
                                        .Select(x => new { x.Label, x.CaseCodeId }).ToArray();
            var codesList = reportCaseCodes.Select(x => x.CaseCodeId).ToArray();
            var ibdList = reportInfo.Columns.Select(x => x.CatalogCodeId).ToArray();
            Expression<Func<ReportEissCode, bool>> whereEntity = x => true;
            if (reportInfo.EntityList.Length > 0)
            {
                whereEntity = x => reportInfo.EntityList.Contains(x.ReportEiss.CourtId);
            }

            var dbData = await repo.AllReadonly<ReportEissCode>()
                                        .Where(x => x.ReportEiss.ReportData.ReportPeriod == reportInfo.PeriodNo)
                                        .Where(x => x.ReportEiss.ReportData.ReportYear == reportInfo.PeriodYear)
                                        .Where(x => x.ReportEiss.ReportData.CatalogId == reportInfo.CatalogId)
                                        .Where(whereEntity)
                                        .Where(x => codesList.Contains(x.CaseCodeId))
                                        .Where(x => ibdList.Contains(x.ReportEiss.CatalogCodeId))
                                        .GroupBy(x => new { x.CaseCodeId, x.ReportEiss.CatalogCodeId })
                                        .Select(x => new
                                        {
                                            CatalogCodeId = x.Key.CaseCodeId,
                                            ReportYear = x.Key.CatalogCodeId,
                                            Count = x.Sum(s => s.CountValue)
                                        }).ToListAsync();

            if (dbData != null && dbData.Count == 0)
            {
                return new DataTablesStatReportVM()
                {
                    NoData = true
                };
            }

            var rowsData = new List<dynamic>();
            int order = 0;
            foreach (var row in reportCaseCodes)
            {
                dynamic rowData = new System.Dynamic.ExpandoObject();
                ((IDictionary<String, object>)rowData)[$"label"] = row.Label;
                ((IDictionary<String, object>)rowData)[$"order"] = ++order;
                foreach (var col in reportInfo.Columns)
                {
                    ((IDictionary<String, object>)rowData)[$"col{col.Id}"] = dbData.Where(x => x.CatalogCodeId == row.CaseCodeId &&
                    x.ReportYear == col.CatalogCodeId).Select(x => x.Count).FirstOrDefault();
                }
                rowsData.Add(rowData);
            }

            var result = initDataTablesModelFromReport(reportInfo, rowsData.ToArray(), $"{reportInfo.PeriodNo.ToBgMonthName()}, {reportInfo.PeriodYear}");
            result.Columns.AddRange(reportInfo.Columns.Select(x => new DataTablesStatReportColumnsVM
            {
                ColumnName = $"col{x.Id}",
                Label = x.Label
            }));
            return result;
        }
        private async Task<DataTablesStatReportVM> eiss_reportDataByCourt(StatReportGetDataVM reportInfo)
        {
            var ibdList = reportInfo.Columns.Select(x => x.CatalogCodeId).ToArray();

            Expression<Func<ReportEissCourt, bool>> whereEntity = x => true;
            if (reportInfo.EntityList.Length > 0)
            {
                whereEntity = x => reportInfo.EntityList.Contains(x.ReportEiss.CourtId);
            }

            var dbData = await repo.AllReadonly<ReportEissCourt>()
                                        .Where(x => x.ReportEiss.ReportData.ReportPeriod == reportInfo.PeriodNo)
                                        .Where(x => x.ReportEiss.ReportData.ReportYear == reportInfo.PeriodYear)
                                        .Where(x => x.ReportEiss.ReportData.CatalogId == reportInfo.CatalogId)
                                        .Where(whereEntity)
                                        .Where(x => ibdList.Contains(x.ReportEiss.CatalogCodeId))
                                        .GroupBy(x => new { x.CourtId, x.ReportEiss.CatalogCodeId })
                                        .Select(x => new
                                        {
                                            CatalogCodeId = x.Key.CourtId,
                                            ReportYear = x.Key.CatalogCodeId,
                                            Count = x.Sum(s => s.CountValue)
                                        }).ToListAsync();

            if (dbData != null && dbData.Count == 0)
            {
                return new DataTablesStatReportVM()
                {
                    NoData = true
                };
            }

            var courtsFromData = dbData.Select(x => x.CatalogCodeId).Distinct().ToArray();
            var courtList = await repo.AllReadonly<CommonCourt>()
                                        .Where(x => courtsFromData.Contains(x.ObjectId))
                                        .Where(x => x.IsActive)
                                        .OrderBy(x => x.Id)
                                        .Select(x => new CommonItemVM
                                        {
                                            Id = x.Id,
                                            Label = x.Label
                                        }).ToListAsync();

            var rowsData = new List<dynamic>();
            int order = 0;
            foreach (var row in courtList)
            {
                dynamic rowData = new System.Dynamic.ExpandoObject();
                ((IDictionary<String, object>)rowData)[$"label"] = row.Label;
                ((IDictionary<String, object>)rowData)[$"order"] = ++order;
                foreach (var col in reportInfo.Columns)
                {
                    ((IDictionary<String, object>)rowData)[$"col{col.Id}"] = dbData.Where(x => x.CatalogCodeId == row.Id &&
                    x.ReportYear == col.CatalogCodeId).Select(x => x.Count).FirstOrDefault();
                }
                rowsData.Add(rowData);
            }

            var result = initDataTablesModelFromReport(reportInfo, rowsData.ToArray(), $"{reportInfo.PeriodNo.ToBgMonthName()}, {reportInfo.PeriodYear}");
            result.Columns.AddRange(reportInfo.Columns.Select(x => new DataTablesStatReportColumnsVM
            {
                ColumnName = $"col{x.Id}",
                Label = x.Label
            }));
            return result;
        }


        #region reportDataByCatalogCode
        /// <summary>
        /// Отчет по дефинирани ИБД показатели, които се визуализират като редове, с 1 колона col1 Общ брой
        /// </summary>
        /// <param name="reportInfo"></param>
        /// <returns></returns>
        private async Task<DataTablesStatReportVM> reportDataByCatalogCode(StatReportGetDataVM reportInfo)
        {
            List<StatReportResultGroupVM> dbData = null;
            switch (reportInfo.IntegrationId)
            {
                case NomenclatureConstants.Integrations.EISS:
                    dbData = await eiss_dbDataByCatalogCode(reportInfo);
                    break;
                case NomenclatureConstants.Integrations.EDIS:
                    dbData = await edis_dbDataByCatalogCode(reportInfo);
                    break;
                case NomenclatureConstants.Integrations.CISSS:
                    dbData = await cis_dbDataByCatalogCode(reportInfo);
                    break;
                case NomenclatureConstants.Integrations.UIS:
                    dbData = await uis_dbDataByCatalogCode(reportInfo);
                    break;
                case NomenclatureConstants.Integrations.EISPP:
                    dbData = await eispp_dbDataByCatalogCode(reportInfo);
                    break;
                case NomenclatureConstants.Integrations.NSI:
                    dbData = await nsi_dbDataByCatalogCode(reportInfo);
                    break;
                default:
                    break;
            }
            if (dbData != null && dbData.Count == 0)
            {
                return new DataTablesStatReportVM()
                {
                    NoData = true
                };
            }

            if (reportInfo.SecondCatalogCodeId > 0 && reportInfo.RatioMultiplier > 0)
            {
                List<StatReportResultGroupVM> secondData = null;
                switch (reportInfo.SecondIntegrationId)
                {
                    case NomenclatureConstants.Integrations.EISS:
                        secondData = await eiss_dbSecondDataByCatalogCode(reportInfo);
                        break;
                    case NomenclatureConstants.Integrations.NSI:
                        secondData = await nsi_dbSecondDataByCatalogCode(reportInfo);
                        break;
                    default:
                        break;
                }
                if (secondData != null)
                {
                    applySecondReportData(reportInfo, dbData, secondData);
                }
            }

            var rowsData = new List<dynamic>();
            int order = 0;
            foreach (var row in reportInfo.Columns)
            {
                dynamic rowData = new System.Dynamic.ExpandoObject();
                ((IDictionary<string, object>)rowData)[$"label"] = $"{row.Label}";
                ((IDictionary<string, object>)rowData)[$"order"] = ++order;

                ((IDictionary<string, object>)rowData)[$"col1"] = dbData.Where(x => x.RowNo == row.CatalogCodeId).Select(x => x.Count).FirstOrDefault();

                rowsData.Add(rowData);
            }

            var result = initDataTablesModelFromReport(reportInfo, rowsData.ToArray(), $"{reportInfo.PeriodNo.ToBgMonthName()}, {reportInfo.PeriodYear}");
            result.Columns.Add(new DataTablesStatReportColumnsVM
            {
                ColumnName = $"col1",
                Label = "Брой"
            });

            return result;
        }

        private void applySecondReportData(StatReportGetDataVM reportInfo, List<StatReportResultGroupVM> data, List<StatReportResultGroupVM> secondData)
        {
            foreach (var d in data)
            {
                var sdCount = secondData.Where(x => x.ColNo == d.ColNo).Sum(x => x.Count);
                if (sdCount > 0 && reportInfo.RatioMultiplier > 0)
                {
                    d.Count = (int)Math.Round((decimal)(d.Count) / sdCount * (decimal)reportInfo.RatioMultiplier);
                }
            }
        }

        private async Task<List<StatReportResultGroupVM>> eiss_dbSecondDataByCatalogCode(StatReportGetDataVM reportInfo)
        {
            Expression<Func<ReportEiss, bool>> whereCourtList = x => true;
            if (reportInfo.EntityList.Length > 0)
            {
                whereCourtList = x => reportInfo.EntityList.Contains(x.CourtId);
            }

            return await repo.AllReadonly<ReportEiss>()
                                        .Where(x => x.ReportData.ReportPeriod == reportInfo.PeriodNo)
                                        .Where(x => x.ReportData.ReportYear == reportInfo.PeriodYear)
                                        .Where(x => x.ReportData.CatalogId == reportInfo.SecondCatalogId)
                                        .Where(whereCourtList)
                                        .Where(x => x.CatalogCodeId == reportInfo.SecondCatalogCodeId)
                                        .GroupBy(x => x.CatalogCodeId)
                                        .Select(x => new StatReportResultGroupVM
                                        {
                                            RowNo = x.Key,
                                            Count = x.Sum(s => s.CountValue)
                                        }).ToListAsync();
        }
        private async Task<List<StatReportResultGroupVM>> nsi_dbSecondDataByCatalogCode(StatReportGetDataVM reportInfo)
        {
            Expression<Func<ReportNsi, bool>> whereEkatteList = x => true;
            if (reportInfo.EntityList.Length > 0)
            {
                int[] ekkateIds;
                switch (reportInfo.IntegrationId)
                {
                    case NomenclatureConstants.Integrations.EISS:
                        ekkateIds = await repo.AllReadonly<CommonCourtEkatte>()
                                            .Where(x => reportInfo.EntityList.Contains(x.CourtId))
                                            .Select(x => x.EkEkatteId).ToArrayAsync();
                        break;
                    case NomenclatureConstants.Integrations.UIS:
                        ekkateIds = await repo.AllReadonly<CommonProsecutorEkatte>()
                                            .Where(x => reportInfo.EntityList.Contains(x.ProsecutorId))
                                            .Select(x => x.EkEkatteId).ToArrayAsync();
                        break;
                    default:
                        ekkateIds = "".ToIntArray();
                        break;
                }


                whereEkatteList = x => ekkateIds.Contains(x.EkatteId);
            }

            return await repo.AllReadonly<ReportNsi>()
                                        .Where(x => x.ReportData.ReportPeriod == reportInfo.PeriodNo)
                                        .Where(x => x.ReportData.ReportYear == reportInfo.PeriodYear)
                                        .Where(x => x.ReportData.CatalogId == reportInfo.SecondCatalogId)
                                        .Where(whereEkatteList)
                                        .Where(x => x.CatalogCodeId == reportInfo.SecondCatalogCodeId)
                                        .GroupBy(x => x.CatalogCodeId)
                                        .Select(x => new StatReportResultGroupVM
                                        {
                                            RowNo = x.Key,
                                            Count = x.Sum(s => s.CountValue)
                                        }).ToListAsync();
        }

        private async Task<List<StatReportResultGroupVM>> eiss_dbSecondDataByCatalogCode3Years(StatReportGetDataVM reportInfo)
        {
            Expression<Func<ReportEiss, bool>> whereCourtList = x => true;
            if (reportInfo.EntityList.Length > 0)
            {

                whereCourtList = x => reportInfo.EntityList.Contains(x.CourtId);
            }

            var lastThreeYearsReports = await getLastThreYearsReportDataIds(reportInfo.SecondCatalogId ?? 0, reportInfo.Last3Years);

            return await repo.AllReadonly<ReportEiss>()
                                        .Where(x => lastThreeYearsReports.Contains(x.ReportDataId))
                                        .Where(whereCourtList)
                                        .Where(x => x.CatalogCodeId == reportInfo.SecondCatalogCodeId)
                                        .GroupBy(x => x.ReportData.ReportYear)
                                        .Select(x => new StatReportResultGroupVM
                                        {
                                            RowNo = x.Key,
                                            Count = x.Sum(s => s.CountValue)
                                        }).ToListAsync();
        }
        private async Task<List<StatReportResultGroupVM>> nsi_dbSecondDataByCatalogCode3Years(StatReportGetDataVM reportInfo)
        {
            Expression<Func<ReportNsi, bool>> whereEkatteList = x => true;
            if (reportInfo.EntityList.Length > 0)
            {
                int[] ekkateIds;
                switch (reportInfo.IntegrationId)
                {
                    case NomenclatureConstants.Integrations.EISS:
                        ekkateIds = await repo.AllReadonly<CommonCourtEkatte>()
                                            .Where(x => reportInfo.EntityList.Contains(x.CourtId))
                                            .Select(x => x.EkEkatteId).ToArrayAsync();
                        break;
                    case NomenclatureConstants.Integrations.UIS:
                        ekkateIds = await repo.AllReadonly<CommonProsecutorEkatte>()
                                            .Where(x => reportInfo.EntityList.Contains(x.ProsecutorId))
                                            .Select(x => x.EkEkatteId).ToArrayAsync();
                        break;
                    default:
                        ekkateIds = "".ToIntArray();
                        break;
                }


                whereEkatteList = x => ekkateIds.Contains(x.EkatteId);
            }

            var lastThreeYearsReports = await getLastThreYearsReportDataIds(reportInfo.SecondCatalogId ?? 0, reportInfo.Last3Years);

            return await repo.AllReadonly<ReportNsi>()
                                        .Where(x => lastThreeYearsReports.Contains(x.ReportDataId))
                                        .Where(whereEkatteList)
                                        .Where(x => x.CatalogCodeId == reportInfo.SecondCatalogCodeId)
                                        .GroupBy(x => x.ReportData.ReportYear)
                                        .Select(x => new StatReportResultGroupVM
                                        {
                                            RowNo = x.Key,
                                            Count = x.Sum(s => s.CountValue)
                                        }).ToListAsync();
        }
        private async Task<List<StatReportResultGroupVM>> eiss_dbDataByCatalogCode(StatReportGetDataVM reportInfo)
        {
            Expression<Func<ReportEiss, bool>> whereEntity = x => true;
            if (reportInfo.EntityList.Length > 0)
            {
                whereEntity = x => reportInfo.EntityList.Contains(x.CourtId);
            }

            return await repo.AllReadonly<ReportEiss>()
                                        .Where(expressionReportDataPeriodYear<ReportEiss>(reportInfo))
                                        .Where(whereEntity)
                                        .Where(x => reportInfo.CatalogCodeIds.Contains(x.CatalogCodeId))
                                        .GroupBy(x => x.CatalogCodeId)
                                        .Select(x => new StatReportResultGroupVM
                                        {
                                            RowNo = x.Key,
                                            Count = x.Sum(s => s.CountValue)
                                        }).ToListAsync();
        }
        private async Task<List<StatReportResultGroupVM>> edis_dbDataByCatalogCode(StatReportGetDataVM reportInfo)
        {
            Expression<Func<ReportEdis, bool>> whereEntity = x => true;
            if (reportInfo.EntityList.Length > 0)
            {
                whereEntity = x => reportInfo.EntityList.Contains(x.CourtId);
            }

            return await repo.AllReadonly<ReportEdis>()
                                        .Where(expressionReportDataPeriodYear<ReportEdis>(reportInfo))
                                        .Where(whereEntity)
                                        .Where(x => reportInfo.CatalogCodeIds.Contains(x.CatalogCodeId))
                                        .GroupBy(x => x.CatalogCodeId)
                                        .Select(x => new StatReportResultGroupVM
                                        {
                                            RowNo = x.Key,
                                            Count = x.Sum(s => s.CountValue)
                                        }).ToListAsync();
        }
        private async Task<List<StatReportResultGroupVM>> cis_dbDataByCatalogCode(StatReportGetDataVM reportInfo)
        {
            Expression<Func<ReportCis, bool>> whereEntity = x => true;
            if (reportInfo.EntityList.Length > 0)
            {
                whereEntity = x => reportInfo.EntityList.Contains(x.InquestId);
            }

            return await repo.AllReadonly<ReportCis>()
                                        .Where(expressionReportDataPeriodYear<ReportCis>(reportInfo))
                                        .Where(whereEntity)
                                        .Where(x => reportInfo.CatalogCodeIds.Contains(x.CatalogCodeId))
                                        .GroupBy(x => x.CatalogCodeId)
                                        .Select(x => new StatReportResultGroupVM
                                        {
                                            RowNo = x.Key,
                                            Count = x.Sum(s => s.CountValue)
                                        }).ToListAsync();
        }
        private async Task<List<StatReportResultGroupVM>> eispp_dbDataByCatalogCode(StatReportGetDataVM reportInfo)
        {
            Expression<Func<ReportEisppMunicipality, bool>> whereEntity = x => true;
            if (reportInfo.EntityList.Length > 0)
            {
                whereEntity = x => reportInfo.EntityList.Contains(x.MunicipalityId);
            }

            return await repo.AllReadonly<ReportEisppMunicipality>()
                                        .Where(expressionReportDataPeriodYear<ReportEisppMunicipality>(reportInfo))
                                        .Where(whereEntity)
                                        .Where(x => reportInfo.CatalogCodeIds.Contains(x.CatalogCodeId))
                                        .GroupBy(x => x.CatalogCodeId)
                                        .Select(x => new StatReportResultGroupVM
                                        {
                                            RowNo = x.Key,
                                            Count = x.Sum(s => s.CountValue)
                                        }).ToListAsync();
        }
        private async Task<List<StatReportResultGroupVM>> uis_dbDataByCatalogCode(StatReportGetDataVM reportInfo)
        {
            Expression<Func<ReportUis, bool>> whereEntity = x => true;
            if (reportInfo.EntityList.Length > 0)
            {
                whereEntity = x => reportInfo.EntityList.Contains(x.ProsecutorId);
            }

            return await repo.AllReadonly<ReportUis>()
                                        .Where(expressionReportDataPeriodYear<ReportUis>(reportInfo))
                                        .Where(whereEntity)
                                        .Where(x => reportInfo.CatalogCodeIds.Contains(x.CatalogCodeId))
                                        .GroupBy(x => x.CatalogCodeId)
                                        .Select(x => new StatReportResultGroupVM
                                        {
                                            RowNo = x.Key,
                                            Count = x.Sum(s => s.CountValue)
                                        }).ToListAsync();
        }
        private async Task<List<StatReportResultGroupVM>> nsi_dbDataByCatalogCode(StatReportGetDataVM reportInfo)
        {
            Expression<Func<ReportNsi, bool>> whereEntity = x => true;
            if (reportInfo.EntityList.Length > 0)
            {
                var ekkateListByMonucipality = await repo.AllReadonly<EkEkatte>()
                                                        .Where(x => reportInfo.EntityList.Contains(x.MunicipalId ?? 0))
                                                        .Select(x => x.Id)
                                                        .ToArrayAsync();
                whereEntity = x => ekkateListByMonucipality.Contains(x.EkatteId);
            }

            return await repo.AllReadonly<ReportNsi>()
                                        .Where(expressionReportDataPeriodYear<ReportNsi>(reportInfo))
                                        .Where(whereEntity)
                                        .Where(x => reportInfo.CatalogCodeIds.Contains(x.CatalogCodeId))
                                        .GroupBy(x => x.CatalogCodeId)
                                        .Select(x => new StatReportResultGroupVM
                                        {
                                            RowNo = x.Key,
                                            Count = x.Sum(s => s.CountValue)
                                        }).ToListAsync();
        }


        Expression<Func<T, bool>> expressionReportDataPeriodYear<T>(StatReportGetDataVM reportInfo) where T : BaseReportDetails
        {
            return x => x.ReportData.CatalogId == reportInfo.CatalogId
                               && x.ReportData.ReportPeriod == reportInfo.PeriodNo
                               && x.ReportData.ReportYear == reportInfo.PeriodYear
                               && x.ReportData.ReportStateId == NomenclatureConstants.ReportStates.Saved;

        }

        #endregion



        private async Task<long[]> getLastThreYearsReportDataIds(int catalogId, int[] years)
        {

            var otherPeriods = repo.AllReadonly<ReportData>()
                                                    .Where(x => x.CatalogId == catalogId)
                                                    .Where(x => years.Contains(x.ReportYear))
                                                    .Select(x => new
                                                    {
                                                        x.ReportPeriod,
                                                        x.ReportYear
                                                    });

            return await repo.AllReadonly<ReportData>()
                                                    .Where(x => x.CatalogId == catalogId)
                                                    .Where(x => years.Contains(x.ReportYear))
                                                    .Where(x => !otherPeriods.Any(p => p.ReportYear == x.ReportYear && p.ReportPeriod > x.ReportPeriod))
                                                    .Select(x => x.Id).ToArrayAsync();
        }



        private async Task<DataTablesStatReportVM> reportData3yearsByCatalogCode(StatReportGetDataVM reportInfo, bool inverseAxis)
        {


            int[] years = reportInfo.Last3Years;


            List<StatReportResultGroupVM> dbData = null;
            switch (reportInfo.IntegrationId)
            {
                case NomenclatureConstants.Integrations.EISS:
                    dbData = await eiss_dbData3yearsByCatalogCode(reportInfo);
                    break;
                case NomenclatureConstants.Integrations.EDIS:
                    dbData = await edis_dbData3yearsByCatalogCode(reportInfo);
                    break;
                case NomenclatureConstants.Integrations.CISSS:
                    dbData = await cis_dbData3yearsByCatalogCode(reportInfo);
                    break;
                case NomenclatureConstants.Integrations.UIS:
                    dbData = await uis_dbData3yearsByCatalogCode(reportInfo);
                    break;
                case NomenclatureConstants.Integrations.EISPP:
                    dbData = await eispp_dbData3yearsByCatalogCode(reportInfo);
                    break;
                case NomenclatureConstants.Integrations.NSI:
                    dbData = await nsi_dbData3yearsByCatalogCode(reportInfo);
                    break;
                default:
                    break;
            }
            if (dbData == null)
                return null;

            if (dbData != null && dbData.Count == 0)
            {
                return new DataTablesStatReportVM()
                {
                    NoData = true
                };
            }

            var rowsData = new List<dynamic>();
            int order = 0;
            DataTablesStatReportVM result = null;
            if (inverseAxis)
            {
                foreach (var row in years.OrderBy(x => x))
                {
                    dynamic rowData = new System.Dynamic.ExpandoObject();
                    ((IDictionary<String, object>)rowData)[$"label"] = $"{row}";
                    ((IDictionary<String, object>)rowData)[$"order"] = ++order;

                    foreach (var col in reportInfo.Columns)
                    {
                        ((IDictionary<String, object>)rowData)[$"col{col.Id}"] = dbData.Where(x => x.ColNo == col.CatalogCodeId &&
                        x.RowNo == row).Select(x => x.Count).FirstOrDefault();
                    }

                    rowsData.Add(rowData);
                }
                result = initDataTablesModelFromReport(reportInfo, rowsData.ToArray(), $"{years.First()}г. - {years.Last()}г.");
                result.Columns.AddRange(reportInfo.Columns.Select(x => new DataTablesStatReportColumnsVM
                {
                    ColumnName = $"col{x.Id}",
                    Label = x.Label
                }));
            }
            else
            {
                foreach (var row in reportInfo.Columns)
                {
                    dynamic rowData = new System.Dynamic.ExpandoObject();
                    ((IDictionary<String, object>)rowData)[$"label"] = $"{row.Label}";
                    ((IDictionary<String, object>)rowData)[$"order"] = ++order;

                    foreach (var col in years.OrderBy(x => x))
                    {
                        ((IDictionary<String, object>)rowData)[$"col{col}"] = dbData.Where(x => x.ColNo == row.CatalogCodeId &&
                        x.RowNo == col).Select(x => x.Count).FirstOrDefault();
                    }

                    rowsData.Add(rowData);
                }
                result = initDataTablesModelFromReport(reportInfo, rowsData.ToArray(), $"{years.First()}г. - {years.Last()}г.");
                result.Columns.AddRange(years.OrderBy(x => x).Select(x => new DataTablesStatReportColumnsVM
                {
                    ColumnName = $"col{x}",
                    Label = x.ToString()
                }));
            }

            return result;
        }
        private async Task<DataTablesStatReportVM> reportData3yearsByCatalogCodeMixed(StatReportGetDataVM reportInfo)
        {
            int[] years = reportInfo.Last3Years;

            List<StatReportResultGroupVM> dbData = null;
            switch (reportInfo.IntegrationId)
            {
                case NomenclatureConstants.Integrations.EISS:
                    dbData = await eiss_dbData3yearsByCatalogCode(reportInfo);
                    break;
                case NomenclatureConstants.Integrations.EDIS:
                    dbData = await edis_dbData3yearsByCatalogCode(reportInfo);
                    break;
                case NomenclatureConstants.Integrations.CISSS:
                    dbData = await cis_dbData3yearsByCatalogCode(reportInfo);
                    break;
                case NomenclatureConstants.Integrations.UIS:
                    dbData = await uis_dbData3yearsByCatalogCode(reportInfo);
                    break;
                case NomenclatureConstants.Integrations.EISPP:
                    dbData = await eispp_dbData3yearsByCatalogCode(reportInfo);
                    break;
                case NomenclatureConstants.Integrations.NSI:
                    dbData = await nsi_dbData3yearsByCatalogCode(reportInfo);
                    break;
                default:
                    break;
            }
            if (dbData == null)
                return null;

            if (dbData != null && dbData.Count == 0)
            {
                return new DataTablesStatReportVM()
                {
                    NoData = true
                };
            }

            List<StatReportResultGroupVM> secondData = null;
            if (reportInfo.SecondCatalogCodeId > 0)
            {

                switch (reportInfo.SecondIntegrationId)
                {

                    case NomenclatureConstants.Integrations.EISS:
                        secondData = await eiss_dbSecondDataByCatalogCode3Years(reportInfo);
                        break;
                    case NomenclatureConstants.Integrations.NSI:
                        secondData = await nsi_dbSecondDataByCatalogCode3Years(reportInfo);
                        break;
                    default:
                        break;
                }
            }

            if (secondData == null)
            {
                return null;
            }


            var rowsData = new List<dynamic>();
            int order = 0;
            DataTablesStatReportVM result = null;

            foreach (var row in years.OrderBy(x => x))
            {
                dynamic rowData = new System.Dynamic.ExpandoObject();
                ((IDictionary<String, object>)rowData)[$"label"] = $"{row}";
                ((IDictionary<String, object>)rowData)[$"order"] = ++order;

                foreach (var col in reportInfo.Columns)
                {
                    ((IDictionary<String, object>)rowData)[$"col{col.Id}"] = dbData.Where(x => x.ColNo == col.CatalogCodeId &&
                    x.RowNo == row).Select(x => x.Count).FirstOrDefault();
                }
                ((IDictionary<String, object>)rowData)[$"addcol"] = secondData.Where(x => x.RowNo == row).Select(x => x.Count).DefaultIfEmpty(0).FirstOrDefault();

                rowsData.Add(rowData);
            }
            result = initDataTablesModelFromReport(reportInfo, rowsData.ToArray(), $"{years.First()}г. - {years.Last()}г.");
            result.Columns.AddRange(reportInfo.Columns.Select(x => new DataTablesStatReportColumnsVM
            {
                ColumnName = $"col{x.Id}",
                Label = x.Label
            }));
            result.Columns.Add(new DataTablesStatReportColumnsVM()
            {
                MixedCol = true,
                ColumnName = "addcol",
                Label = GetPropById<NomCatalogCode, string>(x => x.Id == reportInfo.SecondCatalogCodeId, x => x.Label)
            });


            return result;
        }
        private async Task<List<StatReportResultGroupVM>> eiss_dbData3yearsByCatalogCode(StatReportGetDataVM reportInfo)
        {
            Expression<Func<ReportEiss, bool>> whereEntity = x => true;
            if (reportInfo.EntityList.Length > 0)
            {
                whereEntity = x => reportInfo.EntityList.Contains(x.CourtId);
            }

            var lastThreeYearsReports = await getLastThreYearsReportDataIds(reportInfo.CatalogId, reportInfo.Last3Years);

            return await repo.AllReadonly<ReportEiss>()
                                        .Where(x => lastThreeYearsReports.Contains(x.ReportDataId))
                                        .Where(whereEntity)
                                        .Where(x => reportInfo.CatalogCodeIds.Contains(x.CatalogCodeId))
                                        .GroupBy(x => new { x.ReportData.ReportYear, x.CatalogCodeId })
                                        .Select(x => new StatReportResultGroupVM
                                        {
                                            ColNo = x.Key.CatalogCodeId,
                                            RowNo = x.Key.ReportYear,
                                            Count = x.Sum(s => s.CountValue)
                                        }).ToListAsync();
        }
        private async Task<List<StatReportResultGroupVM>> uis_dbData3yearsByCatalogCode(StatReportGetDataVM reportInfo)
        {
            Expression<Func<ReportUis, bool>> whereEntity = x => true;
            if (reportInfo.EntityList.Length > 0)
            {
                whereEntity = x => reportInfo.EntityList.Contains(x.ProsecutorId);
            }

            var lastThreeYearsReports = await getLastThreYearsReportDataIds(reportInfo.CatalogId, reportInfo.Last3Years);

            return await repo.AllReadonly<ReportUis>()
                                        .Where(x => lastThreeYearsReports.Contains(x.ReportDataId))
                                        .Where(whereEntity)
                                        .Where(x => reportInfo.CatalogCodeIds.Contains(x.CatalogCodeId))
                                        .GroupBy(x => new { x.ReportData.ReportYear, x.CatalogCodeId })
                                        .Select(x => new StatReportResultGroupVM
                                        {
                                            ColNo = x.Key.CatalogCodeId,
                                            RowNo = x.Key.ReportYear,
                                            Count = x.Sum(s => s.CountValue)
                                        }).ToListAsync();
        }

        private async Task<List<StatReportResultGroupVM>> cis_dbData3yearsByCatalogCode(StatReportGetDataVM reportInfo)
        {
            Expression<Func<ReportCis, bool>> whereEntity = x => true;
            if (reportInfo.EntityList.Length > 0)
            {
                whereEntity = x => reportInfo.EntityList.Contains(x.InquestId);
            }

            var lastThreeYearsReports = await getLastThreYearsReportDataIds(reportInfo.CatalogId, reportInfo.Last3Years);

            return await repo.AllReadonly<ReportCis>()
                                        .Where(x => lastThreeYearsReports.Contains(x.ReportDataId))
                                        .Where(whereEntity)
                                        .Where(x => reportInfo.CatalogCodeIds.Contains(x.CatalogCodeId))
                                        .GroupBy(x => new { x.ReportData.ReportYear, x.CatalogCodeId })
                                        .Select(x => new StatReportResultGroupVM
                                        {
                                            ColNo = x.Key.CatalogCodeId,
                                            RowNo = x.Key.ReportYear,
                                            Count = x.Sum(s => s.CountValue)
                                        }).ToListAsync();
        }

        private async Task<List<StatReportResultGroupVM>> edis_dbData3yearsByCatalogCode(StatReportGetDataVM reportInfo)
        {
            Expression<Func<ReportEdis, bool>> whereEntity = x => true;
            if (reportInfo.EntityList.Length > 0)
            {
                whereEntity = x => reportInfo.EntityList.Contains(x.CourtId);
            }

            var lastThreeYearsReports = await getLastThreYearsReportDataIds(reportInfo.CatalogId, reportInfo.Last3Years);

            return await repo.AllReadonly<ReportEdis>()
                                        .Where(x => lastThreeYearsReports.Contains(x.ReportDataId))
                                        .Where(whereEntity)
                                        .Where(x => reportInfo.CatalogCodeIds.Contains(x.CatalogCodeId))
                                        .GroupBy(x => new { x.ReportData.ReportYear, x.CatalogCodeId })
                                        .Select(x => new StatReportResultGroupVM
                                        {
                                            ColNo = x.Key.CatalogCodeId,
                                            RowNo = x.Key.ReportYear,
                                            Count = x.Sum(s => s.CountValue)
                                        }).ToListAsync();
        }

        private async Task<List<StatReportResultGroupVM>> nsi_dbData3yearsByCatalogCode(StatReportGetDataVM reportInfo)
        {
            Expression<Func<ReportNsi, bool>> whereEntity = x => true;
            if (reportInfo.EntityList.Length > 0)
            {
                var ekkateListByMonucipality = await repo.AllReadonly<EkEkatte>()
                                                        .Where(x => reportInfo.EntityList.Contains(x.MunicipalId ?? 0))
                                                        .Select(x => x.Id)
                                                        .ToArrayAsync();
                whereEntity = x => ekkateListByMonucipality.Contains(x.EkatteId);
            }

            var lastThreeYearsReports = await getLastThreYearsReportDataIds(reportInfo.CatalogId, reportInfo.Last3Years);

            return await repo.AllReadonly<ReportNsi>()
                                        .Where(x => lastThreeYearsReports.Contains(x.ReportDataId))
                                        .Where(whereEntity)
                                        .Where(x => reportInfo.CatalogCodeIds.Contains(x.CatalogCodeId))
                                        .GroupBy(x => new { x.ReportData.ReportYear, x.CatalogCodeId })
                                        .Select(x => new StatReportResultGroupVM
                                        {
                                            ColNo = x.Key.CatalogCodeId,
                                            RowNo = x.Key.ReportYear,
                                            Count = x.Sum(s => s.CountValue)
                                        }).ToListAsync();
        }

        private async Task<List<StatReportResultGroupVM>> eispp_dbData3yearsByCatalogCode(StatReportGetDataVM reportInfo)
        {
            Expression<Func<ReportEisppMunicipality, bool>> whereEntity = x => true;
            if (reportInfo.EntityList.Length > 0)
            {
                whereEntity = x => reportInfo.EntityList.Contains(x.MunicipalityId);
            }

            var lastThreeYearsReports = await getLastThreYearsReportDataIds(reportInfo.CatalogId, reportInfo.Last3Years);

            return await repo.AllReadonly<ReportEisppMunicipality>()
                                        .Where(x => lastThreeYearsReports.Contains(x.ReportDataId))
                                        .Where(whereEntity)
                                        .Where(x => reportInfo.CatalogCodeIds.Contains(x.CatalogCodeId))
                                        .GroupBy(x => new { x.ReportData.ReportYear, x.CatalogCodeId })
                                        .Select(x => new StatReportResultGroupVM
                                        {
                                            ColNo = x.Key.CatalogCodeId,
                                            RowNo = x.Key.ReportYear,
                                            Count = x.Sum(s => s.CountValue)
                                        }).ToListAsync();
        }


        /// <summary>
        /// Отчет по дефинирани ИБД показатели по ОСВ, които се визуализират като редове
        /// </summary>
        /// <param name="reportInfo"></param>
        /// <returns></returns>
        private async Task<DataTablesStatReportVM> reportDataByIbdByOSV(StatReportGetDataVM reportInfo)
        {
            List<StatReportResultGroupVM> dbData = null;
            switch (reportInfo.IntegrationId)
            {
                case NomenclatureConstants.Integrations.EISS:
                    dbData = await eiss_dbDataIbdByOSV(reportInfo);
                    break;
                case NomenclatureConstants.Integrations.EDIS:
                    dbData = await edis_dbDataIbdByOSV(reportInfo);
                    break;
                case NomenclatureConstants.Integrations.CISSS:
                    dbData = await cis_dbDataIbdByOSV(reportInfo);
                    break;
                case NomenclatureConstants.Integrations.UIS:
                    dbData = await uis_dbDataIbdByOSV(reportInfo);
                    break;
                case NomenclatureConstants.Integrations.EISPP:
                    dbData = await eispp_dbDataIbdByOSV(reportInfo);
                    break;
                default:
                    break;
            }
            if (dbData != null && dbData.Count == 0)
            {
                return new DataTablesStatReportVM()
                {
                    NoData = true
                };
            }


            var rowsData = new List<dynamic>();
            int order = 0;

            var entityList = await entityListReadLabels(reportInfo);

            foreach (var row in entityList)
            {
                dynamic rowData = new System.Dynamic.ExpandoObject();
                ((IDictionary<String, object>)rowData)[$"label"] = row.Label;
                ((IDictionary<String, object>)rowData)[$"order"] = ++order;
                foreach (var col in reportInfo.Columns)
                {
                    ((IDictionary<String, object>)rowData)[$"col{col.Id}"] = dbData.Where(x => x.RowNo == row.Id &&
                    x.ColNo == col.CatalogCodeId).Select(x => x.Count).FirstOrDefault();
                }
                rowsData.Add(rowData);
            }



            var result = initDataTablesModelFromReport(reportInfo, rowsData.ToArray(), $"{reportInfo.PeriodNo.ToBgMonthName()}, {reportInfo.PeriodYear}");
            result.Columns.AddRange(reportInfo.Columns.Select(x => new DataTablesStatReportColumnsVM
            {
                ColumnName = $"col{x.Id}",
                Label = x.Label
            }));

            return result;
        }

        private async Task<List<StatReportResultGroupVM>> eiss_dbDataIbdByOSV(StatReportGetDataVM reportInfo)
        {
            //Изважда данни само при подадени филтри
            Expression<Func<ReportEiss, bool>> whereEntity = x => false;
            if (reportInfo.EntityList.Length > 0)
            {
                whereEntity = x => reportInfo.EntityList.Contains(x.CourtId);
            }

            return await repo.AllReadonly<ReportEiss>()
                                        .Where(expressionReportDataPeriodYear<ReportEiss>(reportInfo))
                                        .Where(whereEntity)
                                        .Where(x => reportInfo.CatalogCodeIds.Contains(x.CatalogCodeId))
                                        .GroupBy(x => new
                                        {
                                            OsvId = x.CourtId,
                                            x.CatalogCodeId
                                        })
                                        .Select(x => new StatReportResultGroupVM
                                        {
                                            RowNo = x.Key.OsvId,
                                            ColNo = x.Key.CatalogCodeId,
                                            Count = x.Sum(s => s.CountValue)
                                        }).ToListAsync();
        }

        private async Task<List<StatReportResultGroupVM>> eispp_dbDataIbdByOSV(StatReportGetDataVM reportInfo)
        {
            //Изважда данни само при подадени филтри
            Expression<Func<ReportEisppMunicipality, bool>> whereEntity = x => false;
            if (reportInfo.EntityList.Length > 0)
            {
                whereEntity = x => reportInfo.EntityList.Contains(x.MunicipalityId);
            }

            return await repo.AllReadonly<ReportEisppMunicipality>()
                                        .Where(expressionReportDataPeriodYear<ReportEisppMunicipality>(reportInfo))
                                        .Where(whereEntity)
                                        .Where(x => reportInfo.CatalogCodeIds.Contains(x.CatalogCodeId))
                                        .GroupBy(x => new
                                        {
                                            OsvId = x.MunicipalityId,
                                            x.CatalogCodeId
                                        })
                                        .Select(x => new StatReportResultGroupVM
                                        {
                                            RowNo = x.Key.OsvId,
                                            ColNo = x.Key.CatalogCodeId,
                                            Count = x.Sum(s => s.CountValue)
                                        }).ToListAsync();
        }

        private async Task<List<StatReportResultGroupVM>> uis_dbDataIbdByOSV(StatReportGetDataVM reportInfo)
        {
            //Изважда данни само при подадени филтри
            Expression<Func<ReportUis, bool>> whereEntity = x => false;
            if (reportInfo.EntityList.Length > 0)
            {
                whereEntity = x => reportInfo.EntityList.Contains(x.ProsecutorId);
            }

            return await repo.AllReadonly<ReportUis>()
                                        .Where(expressionReportDataPeriodYear<ReportUis>(reportInfo))
                                        .Where(whereEntity)
                                        .Where(x => reportInfo.CatalogCodeIds.Contains(x.CatalogCodeId))
                                        .GroupBy(x => new
                                        {
                                            OsvId = x.ProsecutorId,
                                            x.CatalogCodeId
                                        })
                                        .Select(x => new StatReportResultGroupVM
                                        {
                                            RowNo = x.Key.OsvId,
                                            ColNo = x.Key.CatalogCodeId,
                                            Count = x.Sum(s => s.CountValue)
                                        }).ToListAsync();
        }

        private async Task<List<StatReportResultGroupVM>> edis_dbDataIbdByOSV(StatReportGetDataVM reportInfo)
        {
            //Изважда данни само при подадени филтри
            Expression<Func<ReportEdis, bool>> whereEntity = x => false;
            if (reportInfo.EntityList.Length > 0)
            {
                whereEntity = x => reportInfo.EntityList.Contains(x.CourtId);
            }

            return await repo.AllReadonly<ReportEdis>()
                                        .Where(expressionReportDataPeriodYear<ReportEdis>(reportInfo))
                                        .Where(whereEntity)
                                        .Where(x => reportInfo.CatalogCodeIds.Contains(x.CatalogCodeId))
                                        .GroupBy(x => new
                                        {
                                            OsvId = x.CourtId,
                                            x.CatalogCodeId
                                        })
                                        .Select(x => new StatReportResultGroupVM
                                        {
                                            RowNo = x.Key.OsvId,
                                            ColNo = x.Key.CatalogCodeId,
                                            Count = x.Sum(s => s.CountValue)
                                        }).ToListAsync();
        }

        private async Task<List<StatReportResultGroupVM>> cis_dbDataIbdByOSV(StatReportGetDataVM reportInfo)
        {
            //Изважда данни само при подадени филтри
            Expression<Func<ReportCis, bool>> whereEntity = x => false;
            if (reportInfo.EntityList.Length > 0)
            {
                whereEntity = x => reportInfo.EntityList.Contains(x.InquestId);
            }

            return await repo.AllReadonly<ReportCis>()
                                        .Where(expressionReportDataPeriodYear<ReportCis>(reportInfo))
                                        .Where(whereEntity)
                                        .Where(x => reportInfo.CatalogCodeIds.Contains(x.CatalogCodeId))
                                        .GroupBy(x => new
                                        {
                                            CourtId = x.InquestId,
                                            x.CatalogCodeId
                                        })
                                        .Select(x => new StatReportResultGroupVM
                                        {
                                            RowNo = x.Key.CourtId,
                                            ColNo = x.Key.CatalogCodeId,
                                            Count = x.Sum(s => s.CountValue)
                                        }).ToListAsync();
        }

        private async Task<List<BaseCommonNomenclature>> entityListReadLabels(StatReportGetDataVM reportInfo)
        {
            switch (reportInfo.IntegrationId)
            {
                case NomenclatureConstants.Integrations.EISS:
                case NomenclatureConstants.Integrations.EDIS:
                    return await repo.All<CommonCourt>()
                                    .Where(x => reportInfo.EntityList.Contains(x.Id))
                                    .Where(x => x.IsActive)
                                    .Select(x => new BaseCommonNomenclature
                                    {
                                        Id = x.Id,
                                        Label = x.Label
                                    }).ToListAsync();
                case NomenclatureConstants.Integrations.EISPP:
                case NomenclatureConstants.Integrations.CISSS:
                    return await repo.All<CommonInquest>()
                                    .Where(x => reportInfo.EntityList.Contains(x.Id))
                                    .Where(x => x.IsActive)
                                    .Select(x => new BaseCommonNomenclature
                                    {
                                        Id = x.Id,
                                        Label = x.Label
                                    }).ToListAsync();
                case NomenclatureConstants.Integrations.UIS:
                    return await repo.All<CommonProsecutor>()
                                    .Where(x => reportInfo.EntityList.Contains(x.Id))
                                    .Where(x => x.IsActive)
                                    .Select(x => new BaseCommonNomenclature
                                    {
                                        Id = x.Id,
                                        Label = x.Label
                                    }).ToListAsync();
                default:
                    return new List<BaseCommonNomenclature>();
            }
        }
    }
}


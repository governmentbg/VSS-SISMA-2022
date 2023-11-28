using Microsoft.AspNetCore.Mvc;
using SISMA.Core.Contracts;
using SISMA.Core.Extensions;
using SISMA.Core.Models.Reports;
using SISMA.Core.Models.StatReport;
using SISMA.Infrastructure.Constants;
using SISMA.Infrastructure.Data.Models.Nomenclatures;
using System.Threading.Tasks;

namespace SISMA.Controllers
{
    /// <summary>
    /// Справки по статистически отчети
    /// </summary>
    public class StatReportController : BaseController
    {
        private readonly IStatReportService reportService;
        private readonly INomenclatureService nomService;
        public StatReportController(IStatReportService _reportService, INomenclatureService _nomService)
        {
            reportService = _reportService;
            nomService = _nomService;
            ViewBag.MenuItemValue = "StatReport";
        }

       
        /// <summary>
        /// Основен екран за извикване на отчет
        /// </summary>
        /// <param name="integrationId">код на интеграция</param>
        /// <returns></returns>
        public IActionResult Index(int integrationId = NomenclatureConstants.Integrations.EISS)
        {
            ViewBag.ReportCategoryId_ddl = nomService.GetDropDownListExpr<NomStatReportCategory>(x => x.IntegrationId == integrationId, false);
            ViewBag.PeriodNo_ddl = nomService.GetDDL_Periods();
            ViewBag.PeriodYear_ddl = nomService.GetDDL_Years();
            var model = new StatReportFilterVM()
            {
                IntegrationId = integrationId
            };
            ViewBag.IntegrationLabel = nomService.GetPropById<NomIntegration, string>(x => x.Id == integrationId, x => x.Label);
            return View(model);
        }

        /// <summary>
        /// Зареждане на данни за избран отчет и период
        /// </summary>
        public async Task<IActionResult> GetData(int statReportId, string entityIds, int periodNo, int periodYear)
        {
            return Json(await reportService.Get_ReportData(statReportId, entityIds.ToIntArray(), periodNo, periodYear));
        }

        /// <summary>
        /// Извикване на филтър по институции според вида интеграция
        /// </summary>
        public IActionResult Get_EntityFilter(int integrationId, string selectedContainer)
        {
            ViewBag.ApealRegionId_ddl = nomService.GetDropDownListExpr<NomApealRegion>(x => x.ApealRegionType == NomenclatureConstants.ApealRegionTypes.GetByIntegration(integrationId), false, true);
            ViewBag.DistrictId_ddl = nomService.GetDDL_EkDistricts(false, true);
            var model = new EntitySelectVM()
            {
                IntegrationId = integrationId,
                SelectedContainerUL = selectedContainer,
                ListLabel = NomenclatureConstants.Integrations.EntityLabel(integrationId)
            };

            return PartialView("_EntityFilter", model);
        }
       
    }
}

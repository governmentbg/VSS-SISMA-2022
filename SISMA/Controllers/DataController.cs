using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SISMA.Core.Contracts;
using SISMA.Core.Models.Common;
using SISMA.Core.Models.Reports;
using SISMA.Extensions;
using SISMA.Infrastructure.Constants;
using SISMA.Infrastructure.Data.Models.Nomenclatures;
using System.Collections.Generic;
using System.Linq;

namespace SISMA.Controllers
{
    /// <summary>
    /// Управление на входни данни
    /// </summary>
    public class DataController : BaseController
    {
        private readonly IDataService dataService;
        private readonly INomenclatureService nomService;
        public DataController(
            IDataService _dataService,
            INomenclatureService _nomService)
        {
            dataService = _dataService;
            nomService = _nomService;
        }

        /// <summary>
        /// Входни данни - списък
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var model = new FilterReportData();
            ViewBag.IntegrationId_ddl = nomService.GetDropDownList<NomIntegration>(true, false, true);
            ViewBag.ReportSourceId_ddl = nomService.GetDropDownList<NomReportSource>(true, false, true);
            ViewBag.PeriodNo_ddl = nomService.GetDDL_Periods(true);
            ViewBag.PeriodYear_ddl = nomService.GetDDL_Years(true);
            ViewBag.ReportStateId_ddl = nomService.GetDropDownList<NomReportState>(true, false, true);
            return View(model);
        }

        /// <summary>
        /// Входни данни - зареждане на данни
        /// </summary>
        [HttpPost]
        public IActionResult LoadData(IDataTablesRequest request, FilterReportData filter)
        {
            var data = dataService.Select(filter);
            return request.GetResponse(data);
        }

        /// <summary>
        /// Входни данни - управление на подадени данни за каталог
        /// </summary>
        public IActionResult Manage(long id)
        {
            var model = dataService.Select(new FilterReportData() { Id = id }).FirstOrDefault();
            ViewBag.ReportStateId_ddl = nomService.GetDropDownList<NomReportState>(true, false, true);
            return PartialView(model);
        }

        /// <summary>
        /// Входни данни - управление на подадени данни за каталог - запис
        /// </summary>
        [HttpPost]
        public IActionResult Manage(ReportDataVM model)
        {
            return Json(dataService.Manage(model));
        }

        /// <summary>
        /// Достъпност до съдилища
        /// </summary>
        public IActionResult CityDistance()
        {
            var model = new FilterEkatteItemVM();
            ViewBag.CourtTypes_ddl = nomService.GetDropDownListExpr<NomCourtType>(x => !NomenclatureConstants.CourtTypes.CityDistanseExluded.Contains(x.Id), false, false, false);
            ViewBag.DistanceType_ddl = new List<SelectListItem>()
            {
                new SelectListItem("За населеното място","1"),
                new SelectListItem("За апелативния район","2")
            };
            return View(model);
        }

        /// <summary>
        /// Достъпност до съдилища - зареждане на данни
        /// </summary>
        [HttpPost]
        public IActionResult CityDistance_Data(FilterEkatteItemVM filter)
        {
            var model = dataService.Distance_Select(filter);
            return Json(model);
        }
    }
}

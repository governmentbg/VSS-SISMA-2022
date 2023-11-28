using DataTables.AspNet.Core;
using Microsoft.AspNetCore.Mvc;
using SISMA.Core.Constants;
using SISMA.Core.Contracts;
using SISMA.Core.Models.Reports;
using SISMA.Core.Models.StatReport;
using SISMA.Extensions;
using SISMA.Infrastructure.Constants;
using SISMA.Infrastructure.Contracts;
using SISMA.Infrastructure.Data.Models.Nomenclatures;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SISMA.Controllers
{
    /// <summary>
    /// Управление на статистически отчети
    /// </summary>
    public class NomStatReportController : BaseController
    {
        private readonly INomStatReportService reportService;
        private readonly INomenclatureService nomService;
        public NomStatReportController(INomStatReportService _reportService, INomenclatureService _nomService)
        {
            reportService = _reportService;
            nomService = _nomService;
        }

        #region StatReport

        /// <summary>
        /// Списък отчети по интеграция
        /// </summary>
        /// <param name="integrationId"></param>
        /// <returns></returns>
        public IActionResult ReportIndex(int integrationId)
        {
            var model = new FilterStatReportVM()
            {
                IntegrationId = integrationId
            };
            ViewBag.ReportCategoryId_ddl = nomService.GetDropDownListExpr<NomStatReportCategory>(x => x.IntegrationId == integrationId, true);
            ViewBag.IntegrationLabel = nomService.GetPropById<NomIntegration, string>(x => x.Id == integrationId, x => x.Label);
            ViewBag.CatalogId_ddl = nomService.GetDropDownListExpr<NomCatalog>(x => x.IntegrationId == integrationId, false, true);
            ViewBag.ReportTypeId_ddl = nomService.GetDropDownList<NomStatReportType>(false, true);
            Audit_Operation = NomenclatureConstants.AuditOperations.List;
            Audit_Object = $"Управление на статистически отчети {ViewBag.IntegrationLabel}";
            return View(model);
        }

        /// <summary>
        /// Промяна на подреждането на отчети
        /// </summary>
        /// <param name="id"></param>
        /// <param name="moveUp"></param>
        /// <returns></returns>
        public IActionResult Report_ChangeOrder(int id, bool moveUp)
        {
            return Json(nomService.ChangeOrder_IOrderable<NomStatReport>(moveUp, id));
        }

        /// <summary>
        /// Списък отчети по интеграция - зареждане на данни
        /// </summary>
        [HttpPost]
        public IActionResult Report_LoadData(IDataTablesRequest request, FilterStatReportVM filter)
        {
            var data = reportService.StatReport_Select(filter);
            return request.GetResponse(data);
        }

        public IActionResult ReportAdd(int integrationId)
        {
            var model = new NomStatReport()
            {
                IntegrationId = integrationId,
                DateStart = DateTime.Now,
                IsActive = true
            };
            SetViewBag_Report(model);
            return View(nameof(ReportEdit), model);
        }
        public IActionResult ReportEdit(int id)
        {
            var model = nomService.GetById<NomStatReport>(id);
            SetViewBag_Report(model);
            Audit_Operation = NomenclatureConstants.AuditOperations.View;
            Audit_Object = $"Статистически отчет {model.Label}";

            return View(nameof(ReportEdit), model);
        }

        void SetViewBag_Report(NomStatReport model)
        {
            ViewBag.ReportTypeId_ddl = nomService.GetDropDownList<NomStatReportType>(false);
            if (model.IntegrationId == 0 && model.CatalogId > 0)
            {
                model.IntegrationId = reportService.GetById<NomCatalog>(model.CatalogId).IntegrationId;
            }
            ViewBag.ReportCategoryId_ddl = nomService.GetDropDownListExpr<NomStatReportCategory>(x => x.IntegrationId == model.IntegrationId, false);
            ViewBag.CatalogId_ddl = nomService.GetDropDownListExpr<NomCatalog>(x => x.IntegrationId == model.IntegrationId);
            ViewBag.EntityList = nomService.GetEntityListByIntegration(model.IntegrationId, model.EntityList);

            ViewBag.SecondIntegrationId_ddl = nomService.GetDropDownList<NomIntegration>(true);
            ViewBag.SecondCatalogId_ddl = nomService.GetDropDownListExpr<NomCatalog>(x => x.Id == model.SecondCatalogId, false);
            ViewBag.SecondCatalogCodeId_ddl = nomService.GetDropDownListExpr<NomCatalogCode>(x => x.Id == model.SecondCatalogCodeId, false);
        }



        [HttpPost]
        public async Task<IActionResult> ReportEdit(NomStatReport model)
        {
            SetViewBag_Report(model);
            int currentId = model.Id;
            var result = await reportService.StatReport_SaveData(model);
            if (result.IsSuccessfull)
            {
                Audit_Operation = (currentId == 0) ? NomenclatureConstants.AuditOperations.Add : NomenclatureConstants.AuditOperations.Edit;
                Audit_Object = $"Статистически отчет {model.Label}";

                var eList = nomService.GetEntityListByIntegration(model.IntegrationId, model.EntityList);
                var entityListLog = new LogOperItemModel()
                {
                    Key = "Институции",
                    Items = eList.Select(e => new LogOperItemModel { Value = e.Text }).ToArray()
                };
                SaveLogOperation(currentId, model, model.Id, entityListLog);
                SetSuccessMessage(MessageConstant.Values.SaveOK);
                return RedirectToAction(nameof(ReportEdit), new { id = model.Id });
            }


            SetErrorMessage(MessageConstant.Values.SaveFailed);


            return View(nameof(ReportEdit), model);
        }

        #endregion

        #region StatReportCol

        /// <summary>
        /// Списък ИБД кодове към отчет - зареждане на данни
        /// </summary>
        [HttpPost]
        public IActionResult ReportCol_LoadData(IDataTablesRequest request, int statReportId)
        {
            var data = reportService.StatReportCol_Select(statReportId);
            return request.GetResponse(data);
        }

        /// <summary>
        /// Списък ИБД кодове към отчет - Промяна на подреждането
        /// </summary>
        public IActionResult ReportCol_ChangeOrder(int id, bool moveUp)
        {
            var model = nomService.GetById<NomStatReportCol>(id);
            return Json(nomService.ChangeOrder_IOrderable<NomStatReportCol>(moveUp, id, x => x.StatReportId == model.StatReportId));
        }

        /// <summary>
        /// Списък ИБД кодове към отчет - премахване
        /// </summary>
        public IActionResult ReportCol_Remove(int id)
        {
            var model = nomService.GetById<NomStatReportCol>(id);
            SetViewBag_ReportCol(model);
            var result = nomService.Remove<NomStatReportCol>(id);
            if (result)
            {
                SaveLogOperation(NomenclatureConstants.AuditOperations.Patch, this.ControllerName, nameof(ReportEdit), model, model.StatReportId, new LogOperItemModel() { Value = "Изтрит ИБД код" });
            }
            return Json(result);
        }

        /// <summary>
        /// Списък ИБД кодове към отчет - избор на кодове
        /// </summary>
        public IActionResult ReportColGroup(int statReportId)
        {
            var statReport = reportService.GetById<NomStatReport>(statReportId);
            var model = new ColSelectVM()
            {
                StatReportId = statReportId,
                CatalogId = statReport.CatalogId,
                Label = statReport.Label,
                SelectedList = reportService.StatReportCol_Select(statReportId).Select(x => x.CatalogCodeId.ToString()).ToArray()
            };
            return View(model);
        }
        /// <summary>
        /// Списък ИБД кодове към отчет - избор на кодове - запис на данни
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ReportColGroup(ColSelectVM model)
        {
            var result = await reportService.StatReportCol_SaveData(model);
            if (result.IsSuccessfull)
            {
                SaveLogOperation(NomenclatureConstants.AuditOperations.Patch, this.ControllerName, nameof(ReportEdit), model, model.StatReportId, new LogOperItemModel() { Value = "Актуализирани кодове по ИБД" });
                SetSuccessMessage(MessageConstant.Values.SaveOK);
            }
            else
            {
                SetErrorMessage(MessageConstant.Values.SaveFailed);
            }
            return RedirectToAction(nameof(ReportColGroup), new { statReportId = model.StatReportId });
        }

        /// <summary>
        /// Списък ИБД кодове към отчет - добавяне
        /// </summary>
        public IActionResult ReportColAdd(int statReportId)
        {
            var model = new NomStatReportCol()
            {
                StatReportId = statReportId,
                IsActive = true
            };
            SetViewBag_ReportCol(model);
            return View(nameof(ReportColEdit), model);
        }
        /// <summary>
        /// Списък ИБД кодове към отчет - редакция
        /// </summary>
        public IActionResult ReportColEdit(int id)
        {
            var model = nomService.GetById<NomStatReportCol>(id);
            SetViewBag_ReportCol(model);


            return View(nameof(ReportColEdit), model);
        }

        /// <summary>
        /// Списък ИБД кодове към отчет - запис на данни
        /// </summary>

        [HttpPost]
        public async Task<IActionResult> ReportColEdit(NomStatReportCol model)
        {
            SetViewBag_ReportCol(model);
            int currentId = model.Id;
            var result = await reportService.StatReportCol_SaveData(model);
            if (result.IsSuccessfull)
            {
                SaveLogOperation(currentId, model, model.Id);
                SetSuccessMessage(MessageConstant.Values.SaveOK);
                return Redirect(Url.Action(nameof(ReportEdit), new { id = model.StatReportId }) + "#tabCols");
            }

            SetErrorMessage(result.ErrorMessage);
            return View(nameof(ReportColEdit), model);
        }
        /// <summary>
        /// Списък ИБД кодове към отчет - изчитане на свързани данни
        /// </summary>

        void SetViewBag_ReportCol(NomStatReportCol model)
        {
            var catalogId = reportService.GetPropById<NomStatReport, int>(x => x.Id == model.StatReportId, x => x.CatalogId);
            ViewBag.CatalogCodeId_ddl = nomService.GetDropDownListExpr<NomCatalogCode>(x => x.CatalogId == catalogId, true, false, true, true);
        }


        #endregion

        #region StatReportCaseCode

        /// <summary>
        /// Списък статистически шифри към отчет - зареждане на данни
        /// </summary>
        [HttpPost]
        public IActionResult ReportCaseCode_LoadData(IDataTablesRequest request, int statReportId)
        {
            var data = reportService.StatReportCaseCode_Select(statReportId);
            return request.GetResponse(data);
        }

        /// <summary>
        /// Списък статистически шифри към отчет - промяна в подреждането
        /// </summary>
        public IActionResult ReportCaseCode_ChangeOrder(int id, bool moveUp)
        {
            var model = nomService.GetById<NomStatReportCode>(id);
            return Json(nomService.ChangeOrder_IOrderable<NomStatReportCode>(moveUp, id, x => x.StatReportId == model.StatReportId));
        }

        /// <summary>
        /// Списък статистически шифри към отчет - добавяне
        /// </summary>
        public IActionResult ReportCaseCodeAdd(int statReportId)
        {
            var model = new NomStatReportCode()
            {
                StatReportId = statReportId,
                IsActive = true
            };
            SetViewBag_ReportCaseCode(model);
            return View(nameof(ReportCaseCodeEdit), model);
        }

        /// <summary>
        /// Списък статистически шифри към отчет - редакция
        /// </summary>
        public IActionResult ReportCaseCodeEdit(int id)
        {
            var model = nomService.GetById<NomStatReportCode>(id);
            SetViewBag_ReportCaseCode(model);


            return View(nameof(ReportCaseCodeEdit), model);
        }

        /// <summary>
        /// Списък статистически шифри към отчет - запис на данни
        /// </summary>

        [HttpPost]
        public async Task<IActionResult> ReportCaseCodeEdit(NomStatReportCode model)
        {
            SetViewBag_ReportCaseCode(model);
            int currentId = model.Id;
            var result = await reportService.StatReportCaseCode_SaveData(model);
            if (result.IsSuccessfull)
            {
                SaveLogOperation(currentId, model, model.Id);
                SetSuccessMessage(MessageConstant.Values.SaveOK);
                return Redirect(Url.Action(nameof(ReportEdit), new { id = model.StatReportId }) + "#tabCaseCodes");
            }

            SetErrorMessage(MessageConstant.Values.SaveFailed);
            return View(nameof(ReportCaseCodeEdit), model);
        }

        /// <summary>
        /// Списък статистически шифри към отчет - зареждане на свързани данни
        /// </summary>
        void SetViewBag_ReportCaseCode(NomStatReportCode model)
        {
            var catalogId = reportService.GetById<NomStatReport>(model.StatReportId).CatalogId;
            ViewBag.CaseCodeId_ddl = nomService.GetDropDownListExpr<NomCaseCode>(x => true, true, false, true, true);
            //ViewBag.CaseCodeId_ddl = nomService.GetDropDownList<NomCaseCode>(true);
        }

        #endregion

        #region StatReportCategory

        /// <summary>
        /// Списък категории отчети
        /// </summary>
        public IActionResult ReportCategoryIndex()
        {
            var model = new FilterStatReportCategoryVM();
            SetViewBag_ReportCategory(null);
            return View(model);
        }

        /// <summary>
        /// Списък категории отчети - зареждане на данни
        /// </summary>
        public IActionResult ReportCategory_LoadData(IDataTablesRequest request, FilterStatReportCategoryVM filter)
        {
            var data = reportService.StatReportCategory_Select(filter);
            return request.GetResponse(data);
        }

        /// <summary>
        /// Списък категории отчети - промяна на подреждането
        /// </summary>
        public IActionResult ReportCategory_ChangeOrder(int id, bool moveUp)
        {
            var integrationId = nomService.GetPropById<NomStatReportCategory, int>(x => x.Id == id, x => x.IntegrationId);
            return Json(nomService.ChangeOrder_IOrderable<NomStatReportCategory>(moveUp, id, x => x.IntegrationId == integrationId));
        }

        /// <summary>
        /// Списък категории отчети - добавяне
        /// </summary>

        public IActionResult ReportCategoryAdd()
        {
            var model = new NomStatReportCategory()
            {
                DateStart = DateTime.Now,
                IsActive = true
            };
            SetViewBag_ReportCategory(model);
            return View(nameof(ReportCategoryEdit), model);
        }
        /// <summary>
        /// Списък категории отчети - редакция
        /// </summary>

        public IActionResult ReportCategoryEdit(int id)
        {
            var model = nomService.GetById<NomStatReportCategory>(id);
            SetViewBag_ReportCategory(model);


            return View(nameof(ReportCategoryEdit), model);
        }

        /// <summary>
        /// Списък категории отчети - запис на данни
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ReportCategoryEdit(NomStatReportCategory model)
        {
            int currentId = model.Id;
            var result = await reportService.StatReportCategory_SaveData(model);
            if (result.IsSuccessfull)
            {
                SaveLogOperation(currentId, model, model.Id);
                SetSuccessMessage(MessageConstant.Values.SaveOK);
                return Redirect(Url.Action(nameof(ReportCategoryIndex)));
            }
            SetViewBag_ReportCategory(model);
            SetErrorMessage(MessageConstant.Values.SaveFailed);
            return View(nameof(ReportCategoryEdit), model);
        }

        /// <summary>
        /// Списък категории отчети - зареждане на свързани данни
        /// </summary>
        void SetViewBag_ReportCategory(NomStatReportCategory model)
        {
            ViewBag.IntegrationId_ddl = nomService.GetDropDownList<NomIntegration>(true, false, true);
        }

        #endregion
    }
}

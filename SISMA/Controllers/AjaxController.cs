using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SISMA.Core.Contracts;
using SISMA.Infrastructure.Contracts;
using SISMA.Infrastructure.Data.Models.Nomenclatures;
using System.Linq;

namespace SISMA.Controllers
{
    [Authorize]
    public class AjaxController : Controller
    {
        private readonly INomenclatureService nomenclatureService;

        protected readonly ILogOperationService logOperation;

        public AjaxController(INomenclatureService _nomenclatureService,
            ILogOperationService _logOperation)
        {
            nomenclatureService = _nomenclatureService;
            logOperation = _logOperation;
        }

        /// <summary>
        /// Търсене на населено място по име
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IActionResult SearchEkatte(string query)
        {
            return new JsonResult(nomenclatureService.GetEkatte(query));
        }

        /// <summary>
        /// Зареждане на населено място по ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult GetEkatte(string id)
        {
            var ekatte = nomenclatureService.GetEkatteById(id);

            if (ekatte == null)
            {
                return BadRequest();
            }

            return new JsonResult(ekatte);
        }


        /// <summary>
        /// Търсене/Зареждане на статистически шифър на дело
        /// </summary>
        /// <param name="query"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult SearchCaseCodes(string query, int? id)
        {
            return new JsonResult(nomenclatureService.GetCaseCodes(query, id));
        }

        /// <summary>
        /// Зареждане на институции според интеграция
        /// </summary>
        /// <param name="integrationId"></param>
        /// <param name="idList">Comma separated списък с id-та</param>
        /// <param name="apealRegionId">Код на апелативен район</param>
        /// <param name="districtId">Код на област</param>
        /// <returns></returns>
        public IActionResult GetEntityListByIntegration(int integrationId, string idList, int? apealRegionId = null, int? districtId = null)
        {
            var data = nomenclatureService.GetEntityListByIntegration(integrationId, idList, apealRegionId, districtId);
            return Json(data);
        }

        /// <summary>
        /// Зареждане на статистически отчети по категория
        /// </summary>
        /// <param name="reportCategoryId"></param>
        /// <returns></returns>
        public IActionResult GetStatReportByCategory(int reportCategoryId)
        {
            var data = nomenclatureService.GetDropDownListExpr<NomStatReport>(x => x.ReportCategoryId == reportCategoryId, false);
            return Json(data);
        }

        /// <summary>
        /// Зареждане на каталози по интеграция
        /// </summary>
        /// <param name="integrationId"></param>
        /// <param name="defaultElement"></param>
        /// <returns></returns>
        public IActionResult GetCatalogByIntegration(int integrationId, bool defaultElement = false)
        {
            var data = nomenclatureService.GetDropDownListExpr<NomCatalog>(x => x.IntegrationId == integrationId, defaultElement);
            return Json(data);
        }

        /// <summary>
        /// Зареждане на ИБД кодове от каталог
        /// </summary>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        public IActionResult GetCatalogCodeByCatalog(int catalogId)
        {
            var data = nomenclatureService.GetDropDownListExpr<NomCatalogCode>(x => x.CatalogId == catalogId, false, false, true, true);
            return Json(data);
        }

        #region История на промените

        /// <summary>
        /// Извличане на история на промените за даден обект
        /// </summary>
        /// <param name="controller_name"></param>
        /// <param name="action_name"></param>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public JsonResult Get_LogOperation(string controller_name, string action_name, string objectId)
        {
            var model = logOperation.SelectList(controller_name, action_name, objectId)
                .ToList()
                .Select(i => new
                {
                    oper_date = i.DateWrt.ToString("dd.MM.yyyy HH:mm"),
                    user = i.UserWrt,
                    oper = i.Operation,
                    id = i.Id
                });
            return Json(model);
        }

        /// <summary>
        /// Данни за обект от история на промените
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult Get_LogOperationHTML(long id)
        {
            var data = logOperation.GetUserData(id);
            return Json(data);
        }


        #endregion
    }
}
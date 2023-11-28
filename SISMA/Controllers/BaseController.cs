using Helpers.GenericIO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using SISMA.Components;
using SISMA.Core.Constants;
using SISMA.Core.Contracts;
using SISMA.Infrastructure.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace SISMA.Controllers
{
    /// <summary>
    /// Базов контролер
    /// </summary>
    [Authorize]
    public class BaseController : Controller
    {
        private IUserContext _userContext;

        /// <summary>
        /// Информация за текущия потребител
        /// </summary>
        protected IUserContext userContext
        {
            get
            {
                if (_userContext == null)
                {
                    _userContext = (IUserContext)HttpContext
                         .RequestServices
                         .GetService(typeof(IUserContext));
                }

                return _userContext;
            }
        }


        /// <summary>
        /// Запис на данни в история на промените
        /// </summary>
        /// <param name="currentId"></param>
        /// <param name="model"></param>
        /// <param name="objectId"></param>
        /// <param name="addItem"></param>
        protected void SaveLogOperation(int currentId, object model, object objectId, LogOperItemModel addItem)
        {
            SaveLogOperation(currentId, model, objectId, new List<LogOperItemModel>() { addItem });
        }
        protected void SaveLogOperation(int currentId, object model, object objectId, IList<LogOperItemModel> addItems = null)
        {
            if (currentId == 0)
            {
                SaveLogOperation("Добавяне", model, objectId, addItems);
            }
            else
            {
                SaveLogOperation("Редакция", model, objectId, addItems);
            }
        }

        protected void SaveLogOperation(string oper, object model, object objectId, IList<LogOperItemModel> addItems = null)
        {
            SaveLogOperation(oper, this.ControllerName?.ToLower(), this.ActionName, model, objectId, addItems);
        }
        protected void SaveLogOperation(string oper, string controller, string action, object model, object objectId, LogOperItemModel addItem)
        {
            SaveLogOperation(oper, controller, action, model, objectId, new List<LogOperItemModel>() { addItem });
        }

        protected void SaveLogOperation(string oper, string controller, string action, object model, object objectId, IList<LogOperItemModel> addItems = null)
        {
            ILogOperationService logOperation =
                (ILogOperationService)HttpContext
                .RequestServices
                .GetService(typeof(ILogOperationService));

            var userData = logOperation.GetValuesFromObject(model, ViewData, addItems);

            logOperation.SaveLogOperation(oper, controller, action, objectId.ToString(), userContext.LogName, userData);
        }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            try
            {
                if (filterContext.ActionArguments.Values != null)
                {
                    foreach (var obj in filterContext.ActionArguments.Values.Where(v => v != null && v.GetType()?.IsClass == true))
                    {

                        StringSanitizeHelper.EncodeStringProperties(obj);
                    }
                }
            }
            catch { }


            /*
             *      Управление на активния елемент на менюто
             *      ViewBag.MenuItemValue съдържа ключовата дума, отговорна за отварянето на менюто
             *      Ако не намери атрибут на action-а MenuItem("{keyword}"), се използва името на action-а
             *      Ако action-а е от вида List_Edit се подава list (отрязва до последния символ долна подчертавка)
             */
            ControllerActionDescriptor controllerActionDescriptor = filterContext.ActionDescriptor as ControllerActionDescriptor;

            if (controllerActionDescriptor != null)
            {
                ActionName = controllerActionDescriptor.ActionName;
                ControllerName = controllerActionDescriptor.ControllerName;
                object currentMenuItem = null;
                var menuAttrib = controllerActionDescriptor
                                    .MethodInfo
                                    .CustomAttributes
                                    .FirstOrDefault(a => a.AttributeType == typeof(MenuItemAttribute));
                if (menuAttrib != null)
                {
                    currentMenuItem = menuAttrib.ConstructorArguments[0].Value;
                }
                if (currentMenuItem == null)
                {
                    var actionName = controllerActionDescriptor.ActionName;
                    if (actionName.Contains('_'))
                    {
                        currentMenuItem = actionName.Substring(0, actionName.LastIndexOf('_')).ToLower();
                    }
                    else
                    {
                        currentMenuItem = actionName.ToLower();
                    }
                }
                ViewBag.MenuItemValue = currentMenuItem;
            }
            // ---------Управление на активния елемент на менюто, край

            var tabname = filterContext.HttpContext.Request.Query.Where(x => x.Key == "tabName").Select(x => x.Value).FirstOrDefault();
            if (!string.IsNullOrEmpty(tabname))
            {
                ViewBag.TabName = tabname;
            }
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            lastContext = context;
            lastClientIP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            //if (Request.Headers.TryGetValue("X-Forwarded-For", out var currentIp))
            //{
            //    lastClientIP = currentIp;
            //}

        }

        public void SetSuccessMessage(string message)
        {
            TempData[MessageConstant.SuccessMessage] = message;
        }
        public void SetErrorMessage(string message)
        {
            TempData[MessageConstant.ErrorMessage] = message;
        }
        public void SetWarningMessage(string message)
        {
            TempData[MessageConstant.WarningMessage] = message;
        }

        #region AuditLog
        ActionExecutedContext lastContext;
        string lastClientIP;
        protected string Audit_Operation;
        protected string Audit_Object;
        protected string Audit_Action;
        protected override void Dispose(bool disposing)
        {
            if (!string.IsNullOrEmpty(Audit_Operation))
            {

                string requestUrl = lastContext.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(lastContext.HttpContext.Request.QueryString.Value))
                {
                    requestUrl += lastContext.HttpContext.Request.QueryString.Value;
                }
                if (lastContext.HttpContext.Request.Method != "GET")
                {
                    requestUrl = null;
                }
                var auditService = (IAuditLogService)HttpContext.RequestServices.GetService(typeof(IAuditLogService));
                var auditSave = auditService.SaveAuditLog(Audit_Operation, Audit_Object, lastClientIP, requestUrl, Audit_Action).Result;
            }
            base.Dispose(disposing);
        }
        #endregion

    }
}
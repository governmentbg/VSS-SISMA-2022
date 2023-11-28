using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using SISMA.Infrastructure.Data.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SISMA.Infrastructure.Contracts
{
    public interface ILogOperationService
    {
        LogOperItemModel[] AppendCollection<T>(LogOperItemModel[] logItems, string collectionName, ICollection<T> collection, ViewDataDictionary viewData, params Expression<Func<T, object>>[] props) where T : new();
        LogOperItemModel[] GetUserData(long id);
        string GetValuesFromObject<T>(T obj, ViewDataDictionary viewData, params Expression<Func<T, object>>[] props) where T : new();
        string GetValuesFromObject<T>(T obj, ViewDataDictionary viewData, IList<LogOperItemModel> addItems = null) where T : new();
        void SaveLogOperation(string oper, string controller, string action, string objectId, string userWrt, string userData);
        IQueryable<LogOperListVM> SelectList(string controller, string action, string objectId);
    }
    public class LogOperationService : ILogOperationService
    {
        private readonly IRepository repo;
        public LogOperationService(IRepository _repo)
        {
            repo = _repo;
        }

        private string sanitizeController(string controller)
        {
            var result = controller;
            if (result == null)
            {
                return result;
            }

            result = result.Trim().ToLower().Replace("controller", "");

            return result;
        }

        private string sanitizeAction(string action)
        {
            var result = action;
            if (result == null)
            {
                return result;
            }

            result = result.Trim().ToLower();

            if (result.EndsWith("update"))
            {
                result = result.Substring(0, result.Length - 6);
                result = result + "edit";
            }
            if (result.EndsWith("insert"))
            {
                result = result.Substring(0, result.Length - 6);
                result = result + "edit";
            }

            return result;
        }

        public IQueryable<LogOperListVM> SelectList(string controller, string action, string objectId)
        {
            controller = sanitizeController(controller);
            action = sanitizeAction(action);
            return repo.AllReadonly<LogOperation>()
                            .Where(o => o.Controller == controller && o.Action == action && o.ObjectId == objectId)
                            .OrderByDescending(x => x.Id)
                            .Select(x => new LogOperListVM
                            {
                                Id = x.Id,
                                Operation = x.Operation,
                                UserWrt = x.UserWrt,
                                DateWrt = x.DateWrt
                            }).AsQueryable();
        }

        public void SaveLogOperation(string oper, string controller, string action, string objectId, string userWrt, string userData)
        {
            try
            {
                var newLog = new LogOperation()
                {
                    Operation = oper,
                    Controller = sanitizeController(controller),
                    Action = sanitizeAction(action),
                    ObjectId = objectId,
                    UserWrt = userWrt,
                    DateWrt = DateTime.Now,
                    UserData = userData
                };
                repo.Add(newLog);
                repo.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public LogOperItemModel[] GetUserData(long id)
        {
            var userData = repo.AllReadonly<LogOperation>()
                                .Where(x => x.Id == id)
                                .Select(x => x.UserData)
                                .FirstOrDefault();
            if (string.IsNullOrEmpty(userData))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<LogOperItemModel[]>(userData);
        }

        public string GetValuesFromObject<T>(T obj, ViewDataDictionary viewData, params Expression<Func<T, object>>[] props) where T : new()
        {
            return JsonConvert.SerializeObject(GetKeyValues(obj, viewData, props));
        }
        public LogOperItemModel[] AppendCollection<T>(LogOperItemModel[] logItems, string collectionName, ICollection<T> collection, ViewDataDictionary viewData, params Expression<Func<T, object>>[] props) where T : new()
        {
            var colModel = new LogOperItemModel()
            {
                Key = collectionName
            };

            var result = new List<LogOperItemModel>();
            foreach (var item in collection)
            {
                result.Add(new LogOperItemModel()
                {
                    Key = (result.Count + 1).ToString(),
                    Items = GetKeyValues(item, viewData, props)
                });
            }

            var lItems = logItems.ToList();
            lItems.AddRange(result);
            return lItems.ToArray();
        }

        public LogOperItemModel[] GetKeyValues<T>(T obj, ViewDataDictionary viewData, params Expression<Func<T, object>>[] props) where T : new()
        {
            var result = new List<LogOperItemModel>();
            foreach (var propLambda in props)
            {
                PropertyInfo pInfo = GetPropertyInfo(obj, propLambda);
                result.Add(getValueFromModel(obj, pInfo, viewData));
            }
            return result.ToArray();
        }

        public string GetValuesFromObject<T>(T obj, ViewDataDictionary viewData, IList<LogOperItemModel> addItems = null) where T : new()
        {
            var items = GetKeyValues(obj, viewData).ToList();
            if (addItems != null)
            {
                items.AddRange(addItems);
            }
            return JsonConvert.SerializeObject(items.ToArray());
        }
        public LogOperItemModel[] GetKeyValues<T>(T obj, ViewDataDictionary viewData) where T : new()
        {
            var result = new List<LogOperItemModel>();
            Type objType = obj.GetType();
            var objProperties = objType.GetProperties();
            foreach (var pInfo in objProperties)
            {
                var collectionName = string.Empty;
                var displayName = string.Empty;
                var attribs = pInfo.GetCustomAttributes(true);
                foreach (var attr in attribs.Where(x => x.GetType().Name == typeof(AddToLogAttribute).Name).Select(x => (AddToLogAttribute)x))
                {
                    collectionName = attr.CollectionName;
                    displayName = attr.DisplayName;

                    result.Add(getValueFromModel(obj, pInfo, viewData));
                }

            }
            return result.ToArray();
        }

        private LogOperItemModel getValueFromModel<T>(T obj, PropertyInfo pInfo, ViewDataDictionary viewData) where T : new()
        {
            var displayName = pInfo.GetCustomAttribute<DisplayAttribute>()?.Name ?? pInfo.Name;
            var value = pInfo.GetValue(obj)?.ToString();
            if (viewData != null && viewData[$"{pInfo.Name}_ddl"] != null)
            {
                var ddls = (List<SelectListItem>)viewData[$"{pInfo.Name}_ddl"];
                value = ddls.Where(x => x.Value == value).Select(x => x.Text).FirstOrDefault();
            }
            var propType = pInfo.PropertyType.Name;
            if (propType.Contains("boolean", StringComparison.InvariantCultureIgnoreCase))
            {
                if (pInfo.GetValue(obj) != null)
                {
                    if ((bool)pInfo.GetValue(obj))
                    {
                        value = "[Да]";
                    }
                    else
                    {
                        value = "[не]";
                    }
                }
                else
                {
                    value = "[ ]";
                }
            }
            if (propType.Contains("datetime", StringComparison.InvariantCultureIgnoreCase))
            {
                if (pInfo.GetValue(obj) != null)
                {
                    value = ((DateTime)pInfo.GetValue(obj)).ToString("dd.MM.yyyy");
                }
                else
                {
                    value = "";
                }
            }
            return new LogOperItemModel()
            {
                Key = displayName,
                Value = value
            };
        }

        private PropertyInfo GetPropertyInfo<TSource, TProperty>(TSource source, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression member = propertyLambda.Body as MemberExpression;

            if (member == null)
            {
                var expressionBody = propertyLambda.Body;
                if (expressionBody is UnaryExpression expression && expression.NodeType == ExpressionType.Convert)
                {
                    expressionBody = expression.Operand;
                }
                member = (MemberExpression)expressionBody;
            }

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }
    }

    #region View models
    public class LogOperItemModel
    {
        [JsonProperty("k")]
        public string Key { get; set; }
        [JsonProperty("v")]
        public string Value { get; set; }
        [JsonProperty("i")]
        public LogOperItemModel[] Items { get; set; }
    }

    public class LogOperListVM
    {
        public long Id { get; set; }
        public string Operation { get; set; }
        public string UserWrt { get; set; }
        public DateTime DateWrt { get; set; }
    }

    #endregion

    #region Db Entity

    public class LogOperation
    {
        [Key]
        public long Id { get; set; }
        public string Operation { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string ObjectId { get; set; }
        public string UserWrt { get; set; }
        public DateTime DateWrt { get; set; }
        public string UserData { get; set; }
    }

    public class AddToLogAttribute : Attribute
    {
        public string CollectionName;
        public string DisplayName;
        public AddToLogAttribute()
        {
        }
    }

    #endregion
}

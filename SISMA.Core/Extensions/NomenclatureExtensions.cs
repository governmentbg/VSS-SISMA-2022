using SISMA.Infrastructure.Contracts;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using SISMA.Infrastructure.ViewModels.Common;
using SISMA.Core.Models.Nomenclatures;
using Helpers.GenericIO;

namespace SISMA.Core.Extensions
{
    public static class NomenclatureExtensions
    {
        /// <summary>
        /// Creates SelectList from IQueryable<ICommonNomenclature>
        /// </summary>
        /// <param name="model">Common Nomenclature list of entities</param>
        /// <param name="addDefaultElement">Add 'Choose' to list</param>
        /// <paramref name="addAllElement">Add 'All' to list</param>
        /// <paramref name="orderByNumber">Order by order number</param>
        /// <returns></returns>
        public static List<SelectListItem> ToSelectList(this IQueryable<ICommonNomenclature> model, bool addDefaultElement = false, bool addAllElement = false, bool orderByNumber = true, bool appendCode = false)
        {
            DateTime today = DateTime.Today;

            Expression<Func<ICommonNomenclature, object>> order = x => x.OrderNumber;
            if (!orderByNumber)
            {
                order = x => x.Label;
            }

            var result = model
                .Where(x => x.IsActive)
                .Where(x => x.DateStart <= today)
                .Where(x => (x.DateEnd ?? today) >= today)
                .OrderBy(order)
                .Select(x => new SelectListItem()
                {
                    Text = (appendCode) ? $"{x.Code} {x.Label}" : x.Label,
                    Value = x.Id.ToString()
                }).ToList() ?? new List<SelectListItem>();

            if (addDefaultElement)
            {
                result = result
                    .Prepend(new SelectListItem() { Text = "Избери", Value = "-1" })
                    .ToList();
            }

            if (addAllElement)
            {
                result = result
                    .Prepend(new SelectListItem() { Text = "Всички", Value = "-2" })
                    .ToList();
            }

            return result.Decode();
        }

        public static IQueryable<ICommonNomenclature> FilterActive(this IQueryable<ICommonNomenclature> model)
        {
            DateTime today = DateTime.Today;


            var result = model
                .Where(x => x.IsActive)
                .Where(x => x.DateStart <= today)
                .Where(x => (x.DateEnd ?? today) >= today);

            return result;
        }

        public static List<SelectListItem> Decode(this List<SelectListItem> model)
        {
            foreach (var item in model)
            {
                item.Text = item.Text.Decode();
            }
            return model;
        }

        public static IQueryable<CommonNomenclatureListItem> ToCommonNomenclatureItems(this IQueryable<ICommonNomenclature> model, bool orderByNumber = true)
        {
            DateTime today = DateTime.Today;

            Expression<Func<ICommonNomenclature, object>> order = x => x.OrderNumber;
            if (!orderByNumber)
            {
                order = x => x.Label;
            }

            var result = model
                .FilterActive()
                .OrderBy(order)
                .Select(x => new CommonNomenclatureListItem()
                {
                    Id = x.Id,
                    Code = x.Code,
                    DateStart = x.DateStart,
                    DateEnd = x.DateEnd,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    Label = x.Label,
                    OrderNumber = x.OrderNumber
                });

            return result;
        }

        public static int GetIdByCode(this List<CommonItemVM> model, string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                code = code.Trim();
            }
            return model.Where(x => x.Code == code).Select(x => x.Id).FirstOrDefault();
        }
        public static int GetObjectIdByCode(this List<CommonItemVM> model, string code)
        {
            return model.Where(x => x.Code == code).Select(x => x.ObjectId).FirstOrDefault();
        }

        public static string ToBgMonthName(this int periodNo)
        {
            switch (periodNo)
            {
                case 1: return "Януари";
                case 2: return "Февруари";
                case 3: return "Март";
                case 4: return "Април";
                case 5: return "Май";
                case 6: return "Юни";
                case 7: return "Юли";
                case 8: return "Август";
                case 9: return "Септември";
                case 10: return "Октомври";
                case 11: return "Ноември";
                case 12: return "Декември";
                default:
                    return "-";
            }
        }
    }
}

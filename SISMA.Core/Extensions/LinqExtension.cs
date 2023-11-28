using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SISMA.Core.Extensions
{
    /// <summary>
    /// Разширения на методите на LINQ
    /// </summary>
    public static class LinqExtension
    {

        /// <summary>
        /// Маркира определен елемент, като избран
        /// </summary>
        /// <typeparam name="TSource">Тип на колекцията</typeparam>
        /// <param name="source">Изходна колекция</param>
        /// <param name="selected">Елемент, който да бъде избран</param>
        /// <returns></returns>
        public static SelectList SetSelected<TSource>(
        this IEnumerable<TSource> source, object selected)
        {
            if (source == null)
            {
                return new SelectList(new List<SelectListItem>());
            }
            return new SelectList(source, "Value", "Text", selected);
        }

        /// <summary>
        /// Добавя "Изберете" в SelectList, 
        /// стойността му по подразбиране е -1
        /// </summary>
        /// <param name="source">Изходна колекция</param>
        /// <param name="allText">Текст, по подразбиране "Изберете"</param>
        /// <param name="allValue">Стойност, по подразбиране -1</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> AddAllItem(
         this IEnumerable<SelectListItem> source, string allText = "Изберете", string allValue = "-1")
        {
            var allList = new List<SelectListItem>();
            allList.Add(new SelectListItem() { Text = allText, Value = allValue });
            return allList.Union(source);
        }

        /// <summary>
        /// Добавяне на елемент
        /// </summary>
        /// <param name="source">Изходна колекция</param>
        /// <param name="Text">Име на елемента</param>
        /// <param name="Value">Стойност на елемента</param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> AppendItem(
         this IEnumerable<SelectListItem> source, string Text, string Value)
        {
            var newList = new List<SelectListItem>();
            newList.Add(new SelectListItem() { Text = Text, Value = Value });
            return source.Union(newList);
        }

        /// <summary>
        /// Проверява, дали колекцията е празна
        /// </summary>
        /// <typeparam name="T">Тип на колекцията</typeparam>
        /// <param name="source">Изходна колекция</param>
        /// <returns></returns>
        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            if (source == null)
                return true;

            if (source.Count() == 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Търси съвпадение в текст в базата данни, case sensitive 
        /// </summary>
        /// <typeparam name="T">Тип на таблицата от базата</typeparam>
        /// <param name="source">Колекция от данни</param>
        /// <param name="property">Колона с която сравняваме</param>
        /// <param name="query">Търсен текст</param>
        /// <returns></returns>
        public static IQueryable<T> Like<T>(this IQueryable<T> source, Expression<Func<T, string>> property, string query)
        {
            return LikeInternal(source, property, query, "Like");
        }

        /// <summary>
        /// Търси съвпадение в текст в базата данни, case insensitive 
        /// </summary>
        /// <typeparam name="T">Тип на таблицата от базата</typeparam>
        /// <param name="source">Колекция от данни</param>
        /// <param name="property">Колона с която сравняваме</param>
        /// <param name="query">Търсен текст</param>
        /// <returns></returns>
        public static IQueryable<T> LikeCI<T>(this IQueryable<T> source, Expression<Func<T, string>> property, string query)
        {
            return LikeInternal(source, property, query, "ILike");
        }

        private static IQueryable<T> LikeInternal<T>(IQueryable<T> source, Expression<Func<T, string>> property, string query, string function)
        {
            var selector = property.Body as MemberExpression;

            if (String.IsNullOrEmpty(query) || selector == null || selector.Type != typeof(string))
            {
                return source;
            }

            var parameter = Expression.Parameter(source.ElementType, "x");
            Expression matchExpression = Expression.PropertyOrField(parameter, selector.Member.Name); ;
            var pattern = Expression.Constant($"%{query}%");
            Type dbFunctionType;

            if (function == "Like")
            {
                dbFunctionType = typeof(DbFunctionsExtensions);
            }
            else if (function == "ILike")
            {
                dbFunctionType = typeof(NpgsqlDbFunctionsExtensions);
            }
            else
            {
                return source;
            }

            var filter = Expression.Call(
                dbFunctionType, function, Type.EmptyTypes,
                Expression.Constant(EF.Functions), matchExpression, pattern);

            MethodCallExpression whereCallExpression = Expression.Call(
                typeof(Queryable),
                "Where",
                new Type[] { source.ElementType },
                source.Expression,
                Expression.Lambda<Func<T, bool>>(filter, new ParameterExpression[] { parameter }));

            return source.Provider.CreateQuery<T>(whereCallExpression);
        }

        public static string ToPaternSearch(this string model)
        {
            if (string.IsNullOrWhiteSpace(model))
            {
                return "%";
            }
            return $"%{model.Replace(" ", "%")}%";
        }

        public static int[] ToIntArray(this string model)
        {
            if (string.IsNullOrEmpty(model))
            {
                return (new List<int>()).ToArray();
            }
            try
            {
                return model.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Int32.Parse).ToArray();
            }
            catch
            {
                return (new List<int>()).ToArray();
            }
        }

        public static string EmptyToNull(this string model, string nullVal = "")
        {
            if (model == null || model?.Trim() == nullVal)
            {
                return null;
            }
            return model;
        }

        public static int? EmptyToNull(this int? model, int nullVal = -1)
        {
            if (model == null || model == nullVal)
            {
                return null;
            }
            return model;
        }

        public static DateTime? MakeEndDate(this DateTime? model)
        {
            if (model.HasValue && model.Value.Hour == 0 && model.Value.Minute == 0)
            {
                return model.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            }
            return model;
        }

        public static DateTime MakeEndDate(this DateTime model)
        {
            if (model.Hour == 0 && model.Minute == 0)
            {
                return model.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            }
            return model;
        }
        /// <summary>
        /// Check value is EGN
        /// </summary>
        /// <param name="EGN">string</param>
        /// <returns>string</returns>
        public static bool IsEGN(this string EGN, bool InitiallyValidation = false)
        {
            if (EGN == null) return false;
            if (EGN.Length != 10) return false;
            if (EGN == "0000000000") return false;

            // само първична валидация
            if (InitiallyValidation)
            {
                decimal egn = 0;
                if (!decimal.TryParse(EGN, out egn)) return false;
                return true;
            }

            // пълна валидация
            int a = 0;
            int valEgn = 0;
            for (int i = 0; i < 10; i++)
            {
                if (!int.TryParse(EGN.Substring(i, 1), out a)) return false;
                switch (i)
                {
                    case 0:
                        valEgn += 2 * a;
                        continue;
                    case 1:
                        valEgn += 4 * a;
                        continue;
                    case 2:
                        valEgn += 8 * a;
                        continue;
                    case 3:
                        valEgn += 5 * a;
                        continue;
                    case 4:
                        valEgn += 10 * a;
                        continue;
                    case 5:
                        valEgn += 9 * a;
                        continue;
                    case 6:
                        valEgn += 7 * a;
                        continue;
                    case 7:
                        valEgn += 3 * a;
                        continue;
                    case 8:
                        valEgn += 6 * a;
                        continue;
                }
            }
            long chkSum = valEgn % 11;
            if (chkSum == 10)
                chkSum = 0;
            if (chkSum != Convert.ToInt64(EGN.Substring(9, 1))) return false;
            if ((int.Parse(EGN.Substring(8, 1)) / 2) == 0)
            {
                // girl person
                return true;
            }
            // guy person
            return true;
        }

    }
}

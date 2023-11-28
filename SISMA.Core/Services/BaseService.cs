using Microsoft.Extensions.Logging;
using SISMA.Core.Contracts;
using SISMA.Infrastructure.Data.Common;
using System;
using System.Linq.Expressions;

namespace SISMA.Core.Services
{
    public class BaseService : IBaseService
    {
        protected IRepository repo;
        protected ILogger<BaseService> logger;

        public T GetById<T>(object id) where T : class
        {
            return repo.GetById<T>(id);
        }

        public bool Remove<T>(object id) where T : class
        {
            var model = GetById<T>(id);
            if (model != null)
            {
                repo.Delete(model);
                repo.SaveChanges();
                return true;
            }

            return false;
        }

        public Tprop GetPropById<T, Tprop>(Expression<Func<T, bool>> where, Expression<Func<T, Tprop>> select) where T : class
        {
            return repo.GetPropById<T, Tprop>(where, select);
        }
    }
}

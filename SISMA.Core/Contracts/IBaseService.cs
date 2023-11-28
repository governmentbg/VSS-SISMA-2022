using System;
using System.Linq.Expressions;

namespace SISMA.Core.Contracts
{
    public interface IBaseService
    {
        T GetById<T>(object id) where T : class;

        Tprop GetPropById<T, Tprop>(Expression<Func<T, bool>> where, Expression<Func<T, Tprop>> select) where T : class;
        bool Remove<T>(object id) where T : class;
    }
}

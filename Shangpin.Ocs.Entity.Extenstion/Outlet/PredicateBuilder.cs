using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Shangpin.Ocs.Entity.Extenstion.Outlet
{
    /// <summary>
    /// linq表达式拼接动态查询的类
    /// </summary>
    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> True<T>() { return f => true; }
        public static Expression<Func<T, bool>> False<T>() { return f => false; }
    }
}

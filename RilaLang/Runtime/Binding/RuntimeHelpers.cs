using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Runtime.Binding
{
    public static class RuntimeHelpers
    {
        public static Expression EnsureObjectResult(Expression expr)
        {
            if (!expr.Type.IsValueType)
                return expr;

            if (expr.Type == typeof(void))
                return Expression.Block(expr, Expression.Default(typeof(object)));
            else
                return Expression.Convert(expr, typeof(object));
        }
    }
}

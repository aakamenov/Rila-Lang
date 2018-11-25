using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Scripting.ComInterop;
using Microsoft.Scripting.Utils;

namespace RilaLang.Runtime.Binding
{
    public class RilaBinaryOperationBinder : BinaryOperationBinder
    {
        public RilaBinaryOperationBinder(ExpressionType operation) : base(operation) { }

        public override DynamicMetaObject FallbackBinaryOperation(DynamicMetaObject target, DynamicMetaObject arg, DynamicMetaObject errorSuggestion)
        {
            if (!target.HasValue || !arg.HasValue)
            {
                return Defer(target, arg);
            }

            var restrictions = target.Restrictions
                               .Merge(arg.Restrictions)
                               .Merge(BindingRestrictions.GetTypeRestriction(target.Expression, target.LimitType))
                               .Merge(BindingRestrictions.GetTypeRestriction(arg.Expression, arg.LimitType));

            return new DynamicMetaObject(RuntimeHelpers.EnsureObjectResult(
                        Expression.MakeBinary(Operation,
                                              Expression.Convert(target.Expression, target.LimitType),
                                              Expression.Convert(arg.Expression, arg.LimitType))),restrictions);
        }
    }
}

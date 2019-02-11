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
    public class RilaUnaryOperationBinder : UnaryOperationBinder
    {
        public RilaUnaryOperationBinder(ExpressionType operation) : base(operation) { }

        public override DynamicMetaObject FallbackUnaryOperation(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
        {
            if (!target.HasValue)
                return Defer(target);

            var restrictions = target.Restrictions.Merge(
                BindingRestrictions.GetTypeRestriction(target.Expression, target.LimitType));

            return new DynamicMetaObject(
                RuntimeHelpers.EnsureObjectResult(
                    Expression.MakeUnary(Operation, 
                        Expression.Convert(target.Expression, target.LimitType), 
                    target.LimitType)),
                restrictions);
        }
    }
}

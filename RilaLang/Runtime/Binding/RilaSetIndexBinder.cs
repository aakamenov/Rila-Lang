using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Scripting.ComInterop;
using Microsoft.Scripting.Utils;

namespace RilaLang.Runtime.Binding
{
    public class RilaSetIndexBinder : SetIndexBinder
    {
        public RilaSetIndexBinder(CallInfo callInfo) : base(callInfo) { }

        public override DynamicMetaObject FallbackSetIndex(DynamicMetaObject target, DynamicMetaObject[] indexes, DynamicMetaObject value, DynamicMetaObject errorSuggestion)
        {
            if (ComBinder.TryBindSetIndex(this, target, indexes, value, out DynamicMetaObject result))
                return result;

            if (!target.HasValue || !value.HasValue || indexes.Any(x => !x.HasValue))
                return Defer(target, indexes);

            var arrayAccess = RuntimeHelpers.GetIndexingExpression(target, indexes);
            var setIndex = Expression.Assign(arrayAccess, Expression.Convert(value.Expression, value.LimitType));

            var restrictions = RuntimeHelpers.GetTargetArgsRestrictions(target, indexes, false);

            return new DynamicMetaObject(RuntimeHelpers.EnsureObjectResult(setIndex), restrictions);
        }
    }
}

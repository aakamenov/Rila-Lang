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
    public class RilaGetIndexBinder : GetIndexBinder
    {
        public RilaGetIndexBinder(CallInfo callInfo) : base(callInfo) { }

        public override DynamicMetaObject FallbackGetIndex(DynamicMetaObject target, DynamicMetaObject[] indexes, DynamicMetaObject errorSuggestion)
        {
            if (ComBinder.TryBindGetIndex(this, target, indexes, out DynamicMetaObject result))
                return result;

            if(!target.HasValue || indexes.Any(x => !x.HasValue))
                return Defer(target, indexes);

            var expression = RuntimeHelpers.EnsureObjectResult(RuntimeHelpers.GetIndexingExpression(target,indexes));
            var restrictions = RuntimeHelpers.GetTargetArgsRestrictions(target, indexes, false);

            return new DynamicMetaObject(expression, restrictions);
        }
    }
}

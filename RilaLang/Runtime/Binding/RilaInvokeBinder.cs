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
    public class RilaInvokeBinder : InvokeBinder
    {
        public RilaInvokeBinder(CallInfo callinfo) : base(callinfo) { }

        public override DynamicMetaObject FallbackInvoke(
            DynamicMetaObject target, 
            DynamicMetaObject[] args,
            DynamicMetaObject errorSuggestion)
        {
            // First try COM binding.
            DynamicMetaObject result;
            if (ComBinder.TryBindInvoke(this, target, args, out result))
            {
                return result;
            }
            // Defer if any object has no value so that we evaulate their
            // Expressions and nest a CallSite for the InvokeMember.
            if (!target.HasValue || args.Any((a) => !a.HasValue))
            {
                var deferArgs = new DynamicMetaObject[args.Length + 1];

                for (int i = 0; i < args.Length; i++)
                {
                    deferArgs[i + 1] = args[i];
                }

                deferArgs[0] = target;

                return Defer(deferArgs);
            }
            // Find our own binding.
            if (target.LimitType.IsSubclassOf(typeof(Delegate)))
            {
                var parms = target.LimitType.GetMethod("Invoke").GetParameters();
                if (parms.Length == args.Length)
                {
                    // Don't need to check if argument types match parameters.
                    // If they don't, users get an argument conversion error.
                    var callArgs = RuntimeHelpers.ConvertArguments(args, parms);

                    var expression = Expression.Invoke(
                        Expression.Convert(target.Expression, target.LimitType),
                        callArgs);

                    return new DynamicMetaObject(
                        RuntimeHelpers.EnsureObjectResult(expression),
                        BindingRestrictions.GetTypeRestriction(target.Expression, target.LimitType));
                }
            }

            return errorSuggestion ??
                RuntimeHelpers.CreateThrow(
                    target,
                    args,
                    BindingRestrictions.GetTypeRestriction(target.Expression, target.LimitType),
                    typeof(InvalidOperationException),
                    new string[] { $"Wrong number of arguments passed to function {target.LimitType} got {args}" });

        }
    }
}

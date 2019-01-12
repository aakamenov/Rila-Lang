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
    public class RilaInvokeMemberBinder : InvokeMemberBinder
    {     
        public RilaInvokeMemberBinder(string name, CallInfo callInfo) : base(name, false, callInfo) { }

        public override DynamicMetaObject FallbackInvokeMember(
            DynamicMetaObject target, 
            DynamicMetaObject[] args,
            DynamicMetaObject errorSuggestion)
        {
            // First try COM binding.
            DynamicMetaObject result;

            if (ComBinder.TryBindInvokeMember(this, target, args, out result))
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
            // Could consider allowing invoking static members from an instance.
            var flags = BindingFlags.Instance | BindingFlags.Public;

            var members = target.LimitType.GetMember(Name, flags);

            if ((members.Length == 1) && (members[0] is PropertyInfo || members[0] is FieldInfo))
            {
                // NEED TO TEST, should check for delegate value too
                var mem = members[0];
                throw new NotImplementedException();
                //return new DynamicMetaObject(
                //    Expression.Dynamic(
                //        new RilaInvokeBinder(new CallInfo(args.Length)),
                //        typeof(object),
                //        args.Select(a => a.Expression).AddFirst(
                //               Expression.MakeMemberAccess(this.Expression, mem)));

                // Don't test for eventinfos since we do nothing with them now.
            }
            else
            {
                // Get MethodInfos with right arg counts.
                var methodInfos = members.Where(x => 
                    x.MemberType == MemberTypes.Method && ((MethodInfo)x).GetParameters().Length == args.Length);

                // Get MethodInfos with param types that work for args.  This works
                // except for value args that need to pass to reftype params. 
                // We could detect that to be smarter and then explicitly StrongBox
                // the args.
                List<MethodInfo> res = new List<MethodInfo>();

                foreach (MethodInfo info in methodInfos)
                {
                    if (RuntimeHelpers.ParametersMatchArguments(info.GetParameters(), args))
                    {
                        res.Add(info);
                    }
                }
                // False below means generate a type restriction on the MO.
                // We are looking at the members targetMO's Type.
                var restrictions = RuntimeHelpers.GetTargetArgsRestrictions(target, args, false);

                if (res.Count == 0)
                {
                    return errorSuggestion ??
                        RuntimeHelpers.CreateThrow(target, args, restrictions, typeof(MissingMemberException),
                            new string[] { $"Missing member {args.ToString()}" });
                }
                // restrictions and conversion must be done consistently.
                var callArgs = RuntimeHelpers.ConvertArguments(args, res[0].GetParameters());

                return new DynamicMetaObject(
                   RuntimeHelpers.EnsureObjectResult(
                     Expression.Call(
                        Expression.Convert(target.Expression, target.LimitType),
                        res[0], callArgs)),
                   restrictions);
            }
        }

        public override DynamicMetaObject FallbackInvoke(
            DynamicMetaObject targetMO, 
            DynamicMetaObject[] args,
            DynamicMetaObject errorSuggestion)
        {
            var argexprs = new Expression[args.Length + 1];

            for (int i = 0; i < args.Length; i++)
            {
                argexprs[i + 1] = args[i].Expression;
            }

            argexprs[0] = targetMO.Expression;
            // Just "defer" since we have code in RilaInvokeBinder that knows
            // what to do, and typically this fallback is from a language like
            // Python that passes a DynamicMetaObject with HasValue == false.

            return new DynamicMetaObject(
                           Expression.Dynamic(
                               new RilaInvokeBinder(new CallInfo(args.Length)),
                               typeof(object),
                               argexprs),
                           // No new restrictions since RilaInvokeBinder will handle it.
                           targetMO.Restrictions.Merge(BindingRestrictions.Combine(args)));
        }
    }
}

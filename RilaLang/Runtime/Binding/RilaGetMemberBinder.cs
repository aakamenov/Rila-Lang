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
using RilaLang.Runtime.Binding.Utils;

namespace RilaLang.Runtime.Binding
{
    public class RilaGetMemberBinder : GetMemberBinder
    {
        private readonly Rila runtime;

        public RilaGetMemberBinder(string name, Rila runtime) : base(name, false)
        {
            this.runtime = runtime;
        }

        public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
        {
            // First try COM binding.
            DynamicMetaObject result;

            if (ComBinder.TryBindGetMember(this, target, out result, true))
            {
                return result;
            }

            if (!target.HasValue)
                return Defer(target);

            // Find our own binding.
            var flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public;
            var members = Array.Empty<MemberInfo>();
            var isStatic = false;

            if(target.RuntimeType == typeof(UnresolvedType))
            {
                var unresolved = target.Value as UnresolvedType;

                if (runtime.TypeProvider.IsAlias(unresolved.Name))
                {
                    //Simply pass the name of the member we are trying to access as that is the actual member name
                    return new DynamicMetaObject(
                        Expression.Constant(new UnresolvedType(Name)), BindingRestrictions.Empty);
                }

                if (runtime.TypeProvider.TryGetType(unresolved, out Type type))
                {
                    members = type.GetMember(Name, flags);
                    isStatic = true;
                }
            }
            else
                members = target.LimitType.GetMember(Name, flags);

            if (members?.Length == 1)
            {
                if (isStatic)
                {
                    return new DynamicMetaObject(
                        RuntimeHelpers.EnsureObjectResult(
                            Expression.MakeMemberAccess(null, members[0])),
                        BindingRestrictions.GetTypeRestriction(target.Expression, target.LimitType));
                }
                

                return new DynamicMetaObject(
                    RuntimeHelpers.EnsureObjectResult(
                        Expression.MakeMemberAccess(
                            Expression.Convert(target.Expression, members[0].DeclaringType), members[0])),
                    // Don't need restriction test for name since this
                    // rule is only used where binder is used, which is
                    // only used in sites with this binder.Name.
                    BindingRestrictions.GetTypeRestriction(target.Expression, target.LimitType));
            }
            else
            {
                var error = new string[] { $"Object \"{target.Value.ToString()}\" doesn't contain a member named \"{Name}\"" };

                return errorSuggestion ??
                    RuntimeHelpers.CreateThrow(
                        target, null,
                        BindingRestrictions.GetTypeRestriction(target.Expression, target.LimitType),
                        typeof(MissingMemberException), 
                        error);
            }
        }
    }
}

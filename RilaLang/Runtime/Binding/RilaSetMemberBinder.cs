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
    public class RilaSetMemberBinder : SetMemberBinder
    {
        public RilaSetMemberBinder(string name) : base(name, false) { }

        public override DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value, DynamicMetaObject errorSuggestion)
        {
            if (ComBinder.TryBindSetMember(this, target, value, out DynamicMetaObject result))
                return result;

            if (!target.HasValue)
                return Defer(target);

            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static;
            var members = target.LimitType.GetMember(Name, flags);

            if (members.Length == 1)
            {
                var member = members[0];
                Expression val;
                
                switch(member.MemberType)
                {
                    case MemberTypes.Property:
                        val = Expression.Convert(value.Expression, ((PropertyInfo)member).PropertyType);
                        break;
                    case MemberTypes.Field:
                        val = Expression.Convert(value.Expression, ((FieldInfo)member).FieldType);
                        break;
                    default:
                        {
                            var error = new string[] { "Only property and field setting is supported!" };

                            return errorSuggestion ??
                                    RuntimeHelpers.CreateThrow(target, null,
                                        BindingRestrictions.GetTypeRestriction(target.Expression, target.LimitType),
                                        typeof(InvalidOperationException), error);
                        }

                }              

                return new DynamicMetaObject(
                    RuntimeHelpers.EnsureObjectResult(
                      Expression.Assign(
                        Expression.MakeMemberAccess(
                            Expression.Convert(target.Expression, members[0].DeclaringType),
                            members[0]),
                      val)),                  
                    BindingRestrictions.GetTypeRestriction(target.Expression, target.LimitType));
            }
            else
            {
                var error = new string[] { $"\"{target.RuntimeType}\" contains {members.Length} definitions of \"{Name}\"!" };

                return errorSuggestion ??
                    RuntimeHelpers.CreateThrow(target, null,
                        BindingRestrictions.GetTypeRestriction(target.Expression, target.LimitType),
                        typeof(MissingMemberException), error);
            }
        }
    }
}

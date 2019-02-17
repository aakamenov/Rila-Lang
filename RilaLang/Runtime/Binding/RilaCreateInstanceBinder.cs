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
    public class RilaCreateInstanceBinder : CreateInstanceBinder
    {
        private readonly Rila runtime;

        public RilaCreateInstanceBinder(CallInfo callInfo, Rila runtime) : base(callInfo)
        {
            this.runtime = runtime;
        }

        public override DynamicMetaObject FallbackCreateInstance(DynamicMetaObject target, DynamicMetaObject[] args, DynamicMetaObject errorSuggestion)
        {
            if (!target.HasValue || args.Any(x => !x.HasValue))
                return Defer(target, args);

            ConstructorInfo ctor = null;
            
            if (target.RuntimeType == typeof(UnresolvedType))
            {
                var unresolved = target.Value as UnresolvedType;

                if(runtime.TypeProvider.TryGetType(unresolved, out Type type))
                {
                    ctor = type.GetConstructor(args.Select(x => x.RuntimeType).ToArray());
                }
            }
            else if(target.RuntimeType == typeof(Type))
            {
                var type = target.RuntimeType as Type;
                ctor = type.GetConstructor(args.Select(x => x.RuntimeType).ToArray());
            }

            // We generate an instance restriction on the target since it is a
            // Type and the constructor is associate with the actual Type instance.
            var restrictions = RuntimeHelpers.GetTargetArgsRestrictions(target, args, true);

            if(ctor is null)
            {
                return RuntimeHelpers.CreateThrow(target, args, restrictions,
                    typeof(MissingMemberException),
                    new string[] { $"Could not find a constructor which accepts {args.Length} arguments or it doesn't exist!" });
            }

            var convertedArgs = RuntimeHelpers.ConvertArguments(args, ctor.GetParameters());

            return new DynamicMetaObject(Expression.New(ctor, convertedArgs), restrictions);       
        }
    }
}

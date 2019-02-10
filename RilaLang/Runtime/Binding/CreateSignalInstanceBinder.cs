using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;
using RilaLang.Runtime.Binding.Utils;
using RilaLang.Runtime.Reactive;

namespace RilaLang.Runtime.Binding
{
    public class CreateSignalInstanceBinder : DynamicMetaObjectBinder
    {
        public override Type ReturnType => typeof(Signal<>);

        public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
        {
            if (!target.HasValue || args.Any(x => !x.HasValue))
                return Defer(target, args);

            var restrictions = target.Restrictions.Merge(BindingRestrictions.Combine(args));

            var prevType = args.First().RuntimeType;
            var typesMatch = false;
            
            for(var i = 1; i < args.Length; i++)
            {
                var arg = args[i];

                if (prevType.IsGenericType && prevType.GetGenericTypeDefinition() == typeof(Cell<>))
                {
                    typesMatch = prevType == arg.RuntimeType;

                    if (!typesMatch)
                        break;

                    prevType = arg.RuntimeType;
                }
                else
                {

                    return RuntimeHelpers.CreateThrow(
                        target,
                        args,
                        restrictions,
                        typeof(RilaRuntimeException),
                        new string[] { $"Cells inside the signal expression, denoted with \"@\", must be of type {typeof(Cell<>).Name}" });
                }
            }
            
            if(typesMatch)
            {
                var genericArgument = prevType.GetGenericArguments().First();

                var ctor = typeof(Signal<>).MakeGenericType(genericArgument).GetConstructors().First();
                var listArgType = typeof(IList<>).MakeGenericType(prevType);
                var funcGenericType = typeof(Func<>).MakeGenericType(genericArgument);

                var func = Expression.Lambda(funcGenericType, Expression.Convert(target.Value as Expression, genericArgument));

                var arrayInit = Expression.NewArrayInit(
                    prevType, 
                    args.Select(x => Expression.Convert(x.Expression, prevType)));

                return new DynamicMetaObject(
                    Expression.New(ctor, func, arrayInit),
                    restrictions);
            }

            return RuntimeHelpers.CreateThrow(
                target, 
                args, 
                restrictions, 
                typeof(RilaRuntimeException), 
                new string[] { "All cell types used in the signal expression must match." });
        }
    }
}

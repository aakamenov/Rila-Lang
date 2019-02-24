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

            var cellType = typeof(Cell<>).MakeGenericType(typeof(object));
            var genericArgument = typeof(object);

            var ctor = typeof(Signal<>).MakeGenericType(genericArgument).GetConstructors().First();
            var funcGenericType = typeof(Func<>).MakeGenericType(genericArgument);

            var func = target.Value as Expression<Func<object>>;

            var arrayInit = Expression.NewArrayInit(
                cellType, 
                args.Select(x => Expression.Convert(x.Expression, cellType)));

            return new DynamicMetaObject(
                Expression.New(ctor, func, arrayInit),
                restrictions);
        }
    }
}

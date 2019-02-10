using System;
using System.Linq;
using System.Linq.Expressions;
using System.Dynamic;
using RilaLang.Runtime.Reactive;

namespace RilaLang.Runtime.Binding
{
    public class CreateCellInstanceBinder : DynamicMetaObjectBinder
    {
        public override Type ReturnType => typeof(Cell<>);

        public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
        {
            if (!target.HasValue)
                return Defer(target);

            var ctor = typeof(Cell<>).MakeGenericType(target.RuntimeType).GetConstructors().First();

            return new DynamicMetaObject(
                Expression.New(ctor, 
                    Expression.Constant(target.Value, target.RuntimeType)),
                target.Restrictions);
        }
    }
}

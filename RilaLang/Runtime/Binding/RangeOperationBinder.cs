using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;
using RilaLang.Runtime.Binding.Utils;

namespace RilaLang.Runtime.Binding
{
    public class RangeOperationBinder : DynamicMetaObjectBinder
    {
        public override Type ReturnType => typeof(IEnumerable<>);

        private static readonly Type[] AcceptedTypes = new Type[]
        {
            typeof(int),
            //typeof(double),
            //typeof(float),
            //typeof(decimal),
            //typeof(long),
            //typeof(ulong),
            //typeof(short),
            //typeof(ushort),
            //typeof(uint),
            //typeof(sbyte),
            //typeof(byte)
        };

        public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
        {
            if (!target.HasValue || (args.Length < 1 || !args[0].HasValue))
                return Defer(target, args);

            var end = args[0];

            var restrictions = target.Restrictions
                                     .Merge(end.Restrictions)
                                     .Merge(BindingRestrictions.GetTypeRestriction(target.Expression, target.LimitType))
                                     .Merge(BindingRestrictions.GetTypeRestriction(end.Expression, end.LimitType));

            string errorMessage = null;

            if (target.RuntimeType != end.RuntimeType)
            {
                errorMessage = $"Types provided to operator \"..\" should match. Got \"{target.RuntimeType}\" and \"{end.RuntimeType}\"";
            }
            else if (AcceptedTypes.Contains(target.RuntimeType))
            {
                var ctor = typeof(RangeIterator<>).MakeGenericType(target.RuntimeType).GetConstructors().First();

                return new DynamicMetaObject(
                    Expression.New(
                        ctor, 
                        Expression.Constant(target.Value, target.RuntimeType), 
                        Expression.Constant(end.Value, end.RuntimeType)), 
                    restrictions);
            }
            else
            {
                errorMessage = $"Operator \"..\" only accepts numerical types, but received " +
                               $"\"{target.RuntimeType}\" and \"{end.RuntimeType}\"!";
            }

            var error = new string[] { errorMessage };

            return RuntimeHelpers.CreateThrow(target, args, restrictions, typeof(RuntimeBinderException), error);
        }
    }
}

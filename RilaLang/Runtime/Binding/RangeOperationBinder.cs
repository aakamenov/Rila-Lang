using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;

namespace RilaLang.Runtime.Binding
{
    public class RangeOperationBinder : DynamicMetaObjectBinder
    {
        public override Type ReturnType => typeof(IEnumerable<>);

        private static readonly Type[] AcceptedTypes = new Type[]
        {
            typeof(int),
            typeof(double),
            typeof(float),
            typeof(decimal),
            typeof(long),
            typeof(ulong),
            typeof(short),
            typeof(ushort),
            typeof(uint),
            typeof(sbyte),
            typeof(byte)
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

            if (AcceptedTypes.Contains(target.RuntimeType) && AcceptedTypes.Contains(end.RuntimeType))
            {
                var startValue = (int)target.Value; //TODO: This will throw on values larger than int.MaxValue
                var endValue = (int)end.Value;
                
                if (startValue < 0 || endValue < 0)
                    errorMessage = "Operator \"..\" doesn't accept negative values!";
                else if (endValue < startValue)
                    errorMessage = "Right-hand side argument is less than the left-hand side argument supplied to operator \"..\"";

                if (string.IsNullOrEmpty(errorMessage))
                {
                    var iterations = endValue - startValue;
                    ConstantExpression[] initializers = null;

                    if (iterations > 0)
                    {
                        iterations += 1;
                        var increment = 0;

                        initializers = new ConstantExpression[iterations];

                        for (int i = 0; i < iterations; i++)
                        {
                            initializers[i] = Expression.Constant(startValue + increment++);
                        }
                    }
                    else
                        initializers = Array.Empty<ConstantExpression>();

                    return new DynamicMetaObject(RuntimeHelpers.EnsureObjectResult(
                                    Expression.NewArrayInit(typeof(int), initializers)), restrictions);
                }
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

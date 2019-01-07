using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Microsoft.Scripting.ComInterop;
using Microsoft.Scripting.Utils;

namespace RilaLang.Runtime.Binding
{
    internal static class RuntimeHelpers
    {
        internal static Expression EnsureObjectResult(Expression expr)
        {
            if (!expr.Type.IsValueType)
                return expr;

            if (expr.Type == typeof(void))
                return Expression.Block(expr, Expression.Default(typeof(object)));
            else
                return Expression.Convert(expr, typeof(object));
        }

        internal static DynamicMetaObject CreateThrow(
            DynamicMetaObject target,
            DynamicMetaObject[] args,
            BindingRestrictions restrictions,
            Type exception,
            object[] exceptionArgs)
        {
            if (exception is null)
                throw new ArgumentNullException($"Argument {nameof(exception)} is null!");

            Expression[] argExpressions = null;
            var argTypes = Type.EmptyTypes;

            if (exceptionArgs != null)
            {
                var argsLength = exceptionArgs.Length;
                argExpressions = new Expression[argsLength];
                argTypes = new Type[argsLength];

                for (int i = 0; i < exceptionArgs.Length; i++)
                {
                    var expression = Expression.Constant(exceptionArgs[i]);
                    argExpressions[i] = expression;
                    argTypes[i] = expression.Type;
                }
            }

            var constructor = exception.GetConstructor(argTypes);

            if (constructor is null)
                throw new ArgumentException($"Type {exception.ToString()} coming from argument {nameof(exception)} doesn't contain a constructor with the given signature.");

            return new DynamicMetaObject(Expression.Throw(
                                                          Expression.New(constructor, argExpressions), typeof(object)),
                                         target.Restrictions.Merge(BindingRestrictions.Combine(args).Merge(restrictions)));
        }
    }
}

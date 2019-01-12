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

        public static bool ParametersMatchArguments(ParameterInfo[] parameters, DynamicMetaObject[] args)
        {
            // We only call this after filtering members by this constraint.
            if (args.Length != parameters.Length)
                return false;

            for (int i = 0; i < args.Length; i++)
            {
                var paramType = parameters[i].ParameterType;

                // Could check for HasValue and Value==null AND
                // (paramtype is class or interface) or (is generic
                // and nullable<T>) ... to bind nullables and null.
                if (!paramType.IsAssignableFrom(args[i].LimitType))
                {
                    return false;
                }
            }
            return true;
        }

        public static BindingRestrictions GetTargetArgsRestrictions(
            DynamicMetaObject target,
            DynamicMetaObject[] args,
            bool instanceRestrictionOnTarget)
        {
            // Important to add existing restriction first because the
            // DynamicMetaObjects (and possibly values) we're looking at depend
            // on the pre-existing restrictions holding true.
            var restrictions = target.Restrictions.Merge(BindingRestrictions.Combine(args));

            if (instanceRestrictionOnTarget)
            {
                restrictions = restrictions.Merge(BindingRestrictions.GetInstanceRestriction(
                                                  target.Expression,
                                                  target.Value));
            }
            else
            {
                restrictions = restrictions.Merge(BindingRestrictions.GetTypeRestriction(
                                                  target.Expression,
                                                  target.LimitType));
            }

            for (int i = 0; i < args.Length; i++)
            {
                BindingRestrictions r;

                if (args[i].HasValue && args[i].Value == null)
                {
                    r = BindingRestrictions.GetInstanceRestriction(args[i].Expression, null);
                }
                else
                {
                    r = BindingRestrictions.GetTypeRestriction(args[i].Expression, args[i].LimitType);
                }

                restrictions = restrictions.Merge(r);
            }

            return restrictions;
        }

        public static Expression[] ConvertArguments(DynamicMetaObject[] args, ParameterInfo[] ps)
        {
            Expression[] callArgs = new Expression[args.Length];

            for (int i = 0; i < args.Length; i++)
            {
                Expression argExpr = args[i].Expression;

                argExpr = Expression.Convert(argExpr, ps[i].ParameterType);
                callArgs[i] = argExpr;
            }

            return callArgs;
        }
    }
}

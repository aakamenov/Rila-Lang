using System;
using System.Dynamic;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using RilaLang.Runtime.Binding;
using RilaLang.Compiler.Ast.Utils;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class CallExpression : Expression
    {
        public Expression Function { get; }
        public IReadOnlyCollection<Expression> Arguments { get; }

        public CallExpression(Expression function, IList<Expression> args)
        {
            Function = function;
            Arguments = new ReadOnlyCollection<Expression>(args);
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            var identifier = (IdentifierExpression)Function;

            var args = new DLR.Expression[Arguments.Count];

            for(var i = 0; i < Arguments.Count; i++)
            {
                var arg = Arguments.ElementAt(i);

                if (arg is IdentifierExpression ident) //Argument could be a type itself
                {
                    if (!scope.TryGetVariable(ident.Name, out ParameterExpression _))
                    {
                        args[i] = ExpressionHelpers.ConstructGetTypeExpression(scope, ident.Name);
                        continue;
                    }
                }

                args[i] = arg.GenerateExpressionTree(scope);
            }

            if (scope.Root.FunctionDefinitions.TryGetValue(identifier.Name, out LambdaExpression lambda))
                return DLR.Expression.Invoke(lambda, args);

            return DLR.Expression.Dynamic(new RilaInvokeBinder(new CallInfo(Arguments.Count)), typeof(object), args);
        }
    }
}

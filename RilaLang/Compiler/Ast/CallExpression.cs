using System;
using System.Dynamic;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using RilaLang.Runtime.Binding;

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
                args[i] = Arguments.ElementAt(i).GenerateExpressionTree(scope);
            }

            if (scope.Root.FunctionDefinitions.TryGetValue(identifier.Name, out LambdaExpression lambda))
                return DLR.Expression.Invoke(lambda, args);

            return DLR.Expression.Dynamic(new RilaInvokeBinder(new CallInfo(Arguments.Count)), typeof(object), args);
        }
    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

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

            //TODO: Instead of throwing here, should introduce some sort of a binder
            //in a dynamic expression to look for the definition in the loaded assemblies
            if (!scope.Root.FunctionDefinitions.ContainsKey(identifier.Name))
                throw new InvalidOperationException($"Trying to call \"{identifier.Name}\" which is not defined!");

            var args = new DLR.Expression[Arguments.Count];

            for(var i = 0; i < Arguments.Count; i++)
            {
                args[i] = Arguments.ElementAt(i).GenerateExpressionTree(scope);
            }

            return DLR.Expression.Invoke(scope.Root.FunctionDefinitions[identifier.Name], args);
        }
    }
}

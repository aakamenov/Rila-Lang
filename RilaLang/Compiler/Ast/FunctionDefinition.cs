using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using Microsoft.Scripting;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class FunctionDefinition : Statement
    {
        public string Name { get; }
        public IReadOnlyCollection<IdentifierExpression> Parameters { get; }
        public BlockExpression Body { get; }

        public FunctionDefinition(string name, IList<IdentifierExpression> parameters, BlockExpression body)
        {
            Name = name;
            Parameters = new ReadOnlyCollection<IdentifierExpression>(parameters);
            Body = body;
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            if (scope.IsInLoop() || scope.IsInLambda())
                throw new InvalidOperationException("Cannot declare a function inside the body of another function or inside a loop!");

            if(scope.Root.FunctionDefinitions.ContainsKey(Name))
                throw new InvalidOperationException($"Function \"{Name}\" is already defined!");

            var functionScope = scope.CreateLambda();
            var parameters = new DLR.ParameterExpression[Parameters.Count];

            for(var i = 0; i < Parameters.Count; i++)
            {
                var name = Parameters.ElementAt(i).Name;
                var param = DLR.Expression.Parameter(typeof(object), name);

                parameters[i] = param;
                functionScope.Definitions[name] = param;
            }

            var body = Body.GenerateExpressionTree(functionScope);
            var wrapper = DLR.Expression.Block(body, DLR.Expression.Label(GenScope.ReturnTarget, body));

            var function = DLR.Expression.Lambda(wrapper, Name, parameters);
            scope.Root.FunctionDefinitions[Name] = function;

            GenScope.CreateReturnTarget(); // Reset the label

            return function; //TODO: Since we store the lambda object at the root, this doesn't make sense now...
        }
    }
}

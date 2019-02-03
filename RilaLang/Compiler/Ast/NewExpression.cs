using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using RilaLang.Runtime.Binding.Utils;
using RilaLang.Compiler.Ast.Utils;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class NewExpression : Expression
    {
        public string TypeIdentifier { get; }
        public IReadOnlyCollection<Expression> Arguments { get; }

        public NewExpression(string identifier, IReadOnlyCollection<Expression> arguments)
        {
            TypeIdentifier = identifier;
            Arguments = arguments;
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            var args = new DLR.Expression[Arguments.Count + 1];
            args[0] = DLR.Expression.Constant(new UnresolvedType(TypeIdentifier));

            for (var i = 1; i <= Arguments.Count; i++)
            {
                var arg = Arguments.ElementAt(i - 1);

                if (arg is IdentifierExpression identifier) //Argument could be a type itself
                {
                    if (!scope.TryGetVariable(identifier.Name, out ParameterExpression _))
                    {
                        args[i] = ExpressionHelpers.ConstructGetTypeExpression(scope, identifier.Name);
                        continue;
                    }
                }

                args[i] = arg.GenerateExpressionTree(scope);
            }

            return DLR.Expression.Dynamic(scope.Runtime.GetCreateInstanceBinder(new CallInfo(Arguments.Count)), typeof(object), args);
        }
    }
}

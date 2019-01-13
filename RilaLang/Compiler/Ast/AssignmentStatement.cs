using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class AssignmentStatement : Statement
    {
        public Expression Target { get; }
        public TokenType Operation { get; }
        public Expression Expression { get; }

        public AssignmentStatement(Expression target, TokenType operation, Expression expression)
        {
            Target = target;
            Operation = operation;
            Expression = expression;
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            var expression = Expression.GenerateExpressionTree(scope);

            var identifier = Target as IdentifierExpression;

            if (identifier != null) //Check if a new variable is introduced
            {
                var name = identifier.Name;

                if (!scope.TryGetVariable(name, out DLR.ParameterExpression variable)) //If it is, initialise it
                {
                    variable = DLR.Expression.Variable(typeof(object), name);
                    scope.Definitions[name] = variable;
                }

                return DLR.Expression.Assign(variable, expression);
            }

            var target = Target.GenerateExpressionTree(scope);
            
            switch(Operation)
            {
                case TokenType.Assign:
                    return DLR.Expression.Assign(target, expression);
                default:
                    throw new NotImplementedException(); // Add syntactic sugar operators
            }
        }
    }
}

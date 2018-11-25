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
        public string Identifier { get; private set; }
        public Expression Rhs { get; private set; }

        public AssignmentStatement(string identifier, Expression expression)
        {
            Identifier = identifier;
            Rhs = expression;
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            var variable = DLR.Expression.Parameter(typeof(object), Identifier);

            return DLR.Expression.Block(new DLR.ParameterExpression[] { variable }, Rhs.GenerateExpressionTree(scope));
        }
    }
}

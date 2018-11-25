using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class IndexerExpression : Expression
    {
        public IdentifierExpression Identifier { get; }
        public Expression Expression { get; }

        public IndexerExpression(IdentifierExpression identifier, Expression expression)
        {
            Identifier = identifier;
            Expression = expression;
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            return DLR.Expression.ArrayIndex(Identifier.GenerateExpressionTree(scope), Expression.GenerateExpressionTree(scope));
        }
    }
}

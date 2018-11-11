using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Compiler.Ast
{
    public class IndexerExpression : Expression
    {
        public IdentifierExpression Identifier { get; }
        public Expression Expression { get; }

        public IndexerExpression(IdentifierExpression identifier, Expression expression)
        {
            Identifier = identifier;
            Expression = expression;
        }
    }
}

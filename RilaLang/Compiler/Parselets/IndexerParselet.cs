using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RilaLang.Compiler.Ast;

namespace RilaLang.Compiler.Parselets
{
    public class IndexerParselet : IInfixParselet
    {
        public Precedence Precedence => Precedence.Postfix;

        public Expression Parse(RilaParser parser, Token token, Expression lhs)
        {
            var expression = parser.ParseExpression();

            parser.Expect(out Token rSquare, TokenType.RSquare);

            if (!(lhs is IdentifierExpression))
                parser.AppendError("Expecting an identifier", token);

            return new IndexerExpression(lhs as IdentifierExpression, expression);
        }
    }
}

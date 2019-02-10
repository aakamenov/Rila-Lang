using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RilaLang.Compiler.Ast;

namespace RilaLang.Compiler.Parselets
{
    public class CellParselet : IPrefixParselet
    {
        public Expression Parse(RilaParser parser, Token token)
        {
            parser.Expect(out Token _, TokenType.LParen);

            var expression = parser.ParseExpression();

            parser.Expect(out Token match, TokenType.RParen);

            return new CellExpression(expression);
        }
    }
}

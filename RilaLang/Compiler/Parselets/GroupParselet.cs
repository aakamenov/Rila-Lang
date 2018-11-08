using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RilaLang.Compiler.Ast;

namespace RilaLang.Compiler.Parselets
{
    public class GroupParselet : IPrefixParselet
    {
        public Expression Parse(RilaParser parser, Token token)
        {
            var expression = parser.ParseExpression();
            parser.Expect(out Token match, TokenType.RParen);

            return expression;
        }
    }
}

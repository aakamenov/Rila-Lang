using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RilaLang.Compiler.Ast;

namespace RilaLang.Compiler.Parselets
{
    public class PrefixOperatorParslet : IPrefixParselet
    {
        public Expression Parse(RilaParser parser, Token token)
        {
            var rhs = parser.ParseExpression();

            return new PrefixOperatorExpression(token.TokenType, rhs);
        }
    }
}

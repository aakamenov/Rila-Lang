using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RilaLang.Compiler.Ast;

namespace RilaLang.Compiler.Parselets
{
    public class PrefixOperatorParselet : IPrefixParselet, IPrecedence
    {
        public Precedence Precedence { get; }

        public PrefixOperatorParselet(Precedence precedence)
        {
            Precedence = precedence;
        }

        public Expression Parse(RilaParser parser, Token token)
        {
            var rhs = parser.ParseExpression(Precedence);

            return new PrefixOperatorExpression(token.TokenType, rhs);
        }
    }
}

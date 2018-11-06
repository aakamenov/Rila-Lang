using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RilaLang.Compiler.Ast;

namespace RilaLang.Compiler.Parselets
{
    public class BinaryOperatorParslet : IInfixParselet
    {
        public Expression Parse(RilaParser parser, Token token, Expression lhs)
        {
            var right = parser.ParseExpression();

            return new BinaryOperatorExpression(lhs, token.TokenType, right);
        }
    }
}

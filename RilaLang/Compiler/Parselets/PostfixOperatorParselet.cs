using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RilaLang.Compiler.Ast;

namespace RilaLang.Compiler.Parselets
{
    public class PostfixOperatorParselet : IInfixParselet
    {
        public Precedence Precedence { get; }

        public PostfixOperatorParselet(Precedence precedence)
        {
            Precedence = precedence;
        }

        public Expression Parse(RilaParser parser, Token token, Expression lhs)
        {
            return new PostfixOperatorExpression(lhs, token.TokenType);
        }
    }
}

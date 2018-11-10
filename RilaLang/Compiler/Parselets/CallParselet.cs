using System.Collections.Generic;
using RilaLang.Compiler.Ast;

namespace RilaLang.Compiler.Parselets
{
    public class CallParselet : IInfixParselet
    {
        public Precedence Precedence => Precedence.Parentheses;

        public Expression Parse(RilaParser parser, Token token, Expression lhs)
        {
            var args = new List<Expression>();

            if (!parser.ConsumeIf(TokenType.RParen))
            {
                do
                {
                    args.Add(parser.ParseExpression());
                }
                while (parser.ConsumeIf(TokenType.Comma));

                parser.Expect(out Token match, TokenType.RParen);
            }

            return new CallExpression(lhs, args);
        }
    }
}

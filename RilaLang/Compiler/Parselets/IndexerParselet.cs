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
            var parameters = new List<Expression>();

            if (!parser.ConsumeIf(TokenType.RSquare))
            {
                do
                {
                    parameters.Add(parser.ParseExpression());
                }
                while (parser.ConsumeIf(TokenType.Comma));

                parser.Expect(out Token rSquare, TokenType.RSquare);
            }

            return new IndexerExpression(lhs, parameters);
        }
    }
}

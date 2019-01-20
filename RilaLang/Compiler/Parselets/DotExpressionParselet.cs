using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RilaLang.Compiler.Ast;

namespace RilaLang.Compiler.Parselets
{
    public class DotExpressionParselet : IInfixParselet
    {
        public Precedence Precedence => Precedence.Postfix;

        public Expression Parse(RilaParser parser, Token token, Expression lhs)
        {
            var expressions = new List<Expression>() { lhs };

            do
            {
                expressions.Add(parser.ParseExpression(Precedence));
            }
            while (parser.ConsumeIf(TokenType.Dot));

            if(parser.ConsumeIf(TokenType.Assign))
            {
                expressions.Add(parser.ParseExpression());

                return new DotExpression(expressions, true);
            }

            return new DotExpression(expressions, false);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RilaLang.Compiler.Ast;

namespace RilaLang.Compiler.Parselets
{
    public class SignalParselet : IPrefixParselet
    {
        public Expression Parse(RilaParser parser, Token token)
        {
            var distance = 0;
            var next = parser.Peek(distance);

            var cellNames = new List<string>();

            while(true)
            {
                if(next.TokenType == TokenType.EOF)
                {
                    parser.AppendError("Missing closing \"}\".", next);
                    break;
                }

                if (next.TokenType == TokenType.RCurly)
                    break;

                if(next.TokenType == TokenType.At)
                {
                    var identifier = parser.Peek(++distance);

                    if (identifier.TokenType != TokenType.Identifier)
                    {
                        parser.AppendError($"Expecting a cell variable identifier. Got: \"{identifier.Content}\"", identifier);
                        continue;
                    }

                    cellNames.Add(identifier.Content);
                }

                next = parser.Peek(++distance);
            }

            var expression = parser.ParseExpression();
            parser.Expect(out Token _, TokenType.RCurly);

            return new SignalExpression(expression, cellNames);
        }
    }
}

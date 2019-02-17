using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RilaLang.Compiler.Ast;

namespace RilaLang.Compiler.Parselets
{
    public class IdentifierParselet : IPrefixParselet
    {
        public Expression Parse(RilaParser parser, Token token)
        {
            if(token.TokenType == TokenType.At)
            {
                token = parser.Consume();

                if(token.TokenType != TokenType.Identifier)
                    parser.AppendError($"Expecting a cell variable identifier. Got: \"{token.Content}\"", token);
            }

            return new IdentifierExpression(token.Content);
        }
    }
}

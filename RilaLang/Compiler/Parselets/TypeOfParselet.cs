using RilaLang.Compiler.Ast;

namespace RilaLang.Compiler.Parselets
{
    public class TypeOfParselet : IPrefixParselet
    {
        public Expression Parse(RilaParser parser, Token token)
        {
            parser.Expect(out Token _, TokenType.LParen);
            parser.Expect(out Token identifier, TokenType.Identifier);
            parser.Expect(out Token _, TokenType.RParen);

            return new TypeOfExpression(identifier.Content);
        }
    }
}

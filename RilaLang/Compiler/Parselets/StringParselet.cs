using RilaLang.Compiler.Ast;

namespace RilaLang.Compiler.Parselets
{
    public class StringParselet : IPrefixParselet
    {
        public Expression Parse(RilaParser parser, Token token)
        {
            return new StringExpression(token.Content);
        }
    }
}

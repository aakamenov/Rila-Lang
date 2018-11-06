using RilaLang.Compiler.Ast;

namespace RilaLang.Compiler.Parselets
{
    public class BoolParselet : IPrefixParselet
    {
        public Expression Parse(RilaParser parser, Token token)
        {
            return new BoolExpression(bool.Parse(token.Content));
        }
    }
}

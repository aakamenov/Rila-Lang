using RilaLang.Compiler.Ast;

namespace RilaLang.Compiler.Parselets
{
    public class NumberParselet : IPrefixParselet
    {
        public Expression Parse(RilaParser parser, Token token)
        {
            return new NumberExpression(int.Parse(token.Content));
        }
    }
}

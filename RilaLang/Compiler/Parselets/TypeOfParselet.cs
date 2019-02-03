using RilaLang.Compiler.Ast;

namespace RilaLang.Compiler.Parselets
{
    public class TypeOfParselet : IPrefixParselet
    {
        public Expression Parse(RilaParser parser, Token token)
        {
            return new TypeOfExpression(parser.ParseExpression());
        }
    }
}

using RilaLang.Compiler.Ast;

namespace RilaLang.Compiler.Parselets
{
    public interface IInfixParselet : IPrecedence
    {
        Expression Parse(RilaParser parser, Token token, Expression lhs);
    }
}

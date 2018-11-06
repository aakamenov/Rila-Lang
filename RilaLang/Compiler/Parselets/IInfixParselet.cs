using RilaLang.Compiler.Ast;

namespace RilaLang.Compiler.Parselets
{
    public interface IInfixParselet
    {
        Precedence Precedence { get; }
        Expression Parse(RilaParser parser, Token token, Expression lhs);
    }
}

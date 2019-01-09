using RilaLang.Compiler.Ast;

namespace RilaLang.Compiler.Parselets
{
    public class RangeOperatorParselet : IInfixParselet
    {
        public Precedence Precedence => Precedence.Equality; //This way we can include nested expressions

        public Expression Parse(RilaParser parser, Token token, Expression lhs)
        {
            var right = parser.ParseExpression(Precedence);

            return new RangeExpression(lhs, right);
        }
    }
}

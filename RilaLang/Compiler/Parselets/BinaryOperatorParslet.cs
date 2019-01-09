using RilaLang.Compiler.Ast;

namespace RilaLang.Compiler.Parselets
{
    public class BinaryOperatorParslet : IInfixParselet
    {
        public Precedence Precedence { get; }

        public BinaryOperatorParslet(Precedence precedence)
        {
            Precedence = precedence;
        }

        public Expression Parse(RilaParser parser, Token token, Expression lhs)
        {
            var right = parser.ParseExpression(Precedence);

            return new BinaryOperatorExpression(lhs, token.TokenType, right);
        }
    }
}

using System;

namespace RilaLang.Compiler.Ast
{
    public class PrefixOperatorExpression : Expression
    {
        public TokenType Operation { get; }
        public Expression Rhs { get; }

        internal PrefixOperatorExpression(TokenType operation, Expression rhs)
        {
            Operation = operation;
            Rhs = rhs;
        }
    }
}

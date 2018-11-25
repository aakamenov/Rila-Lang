using System;
using System.Linq.Expressions;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class PrefixOperatorExpression : Expression
    {
        public TokenType Operation { get; }
        public Expression Rhs { get; }

        internal PrefixOperatorExpression(TokenType operation, Expression rhs)
        {
            Operation = operation;
            Rhs = rhs;
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            switch(Operation)
            {
                case TokenType.Minus: return DLR.Expression.MakeUnary(ExpressionType.Negate, Rhs.GenerateExpressionTree(scope), typeof(object));
                case TokenType.Plus: return DLR.Expression.MakeUnary(ExpressionType.UnaryPlus, Rhs.GenerateExpressionTree(scope), typeof(object));
                case TokenType.Not: return DLR.Expression.MakeUnary(ExpressionType.Not, Rhs.GenerateExpressionTree(scope), typeof(object));
                default:
                    throw new InvalidOperationException("Invalid token representing a prefix operation.");
            }
        }
    }
}

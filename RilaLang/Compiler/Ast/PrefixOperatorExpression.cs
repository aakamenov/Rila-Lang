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
            var rhs = Rhs.GenerateExpressionTree(scope);

            return DLR.Expression.Dynamic(scope.Runtime.GetUnaryOperationBinder(GetOperation()), typeof(object), rhs);
        }

        private ExpressionType GetOperation()
        {
            switch(Operation)
            {
                case TokenType.Minus: return ExpressionType.Negate;
                case TokenType.Plus: return ExpressionType.UnaryPlus;
                case TokenType.Not: return ExpressionType.Not;
                default:
                    throw new InvalidOperationException("Invalid token representing an unary operation.");
            }
        }
    }
}

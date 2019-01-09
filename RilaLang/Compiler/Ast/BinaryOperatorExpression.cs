using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using RilaLang.Runtime.Binding;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class BinaryOperatorExpression : Expression
    {
        public Expression Lhs { get; }
        public Expression Rhs { get; }
        public TokenType Operation { get; }

        public BinaryOperatorExpression(Expression lhs, TokenType operation, Expression rhs)
        {
            Lhs = lhs;
            Operation = operation;
            Rhs = rhs;
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            var lhs = Lhs.GenerateExpressionTree(scope);
            var rhs = Rhs.GenerateExpressionTree(scope);
            var op = GetOperation();
            
            return DLR.Expression.Dynamic(scope.Runtime.GetBinaryOperationBinder(op), typeof(object), lhs, rhs);
        }

        private ExpressionType GetOperation()
        {
            switch (Operation)
            {
                case TokenType.Minus: return ExpressionType.Subtract;
                case TokenType.Plus: return ExpressionType.Add;
                case TokenType.Asterisk: return ExpressionType.Multiply;
                case TokenType.Slash: return ExpressionType.Divide;
                case TokenType.Modulo: return ExpressionType.Modulo;
                case TokenType.And: return ExpressionType.And;
                case TokenType.Or: return ExpressionType.Or;
                case TokenType.GreaterThan: return ExpressionType.GreaterThan;
                case TokenType.EqGreaterThan: return ExpressionType.GreaterThanOrEqual;
                case TokenType.LessThan: return ExpressionType.LessThan;
                case TokenType.EqLessThan: return ExpressionType.LessThanOrEqual;
                case TokenType.Equal: return ExpressionType.Equal;
                case TokenType.NotEqual: return ExpressionType.NotEqual;
                default:
                    throw new InvalidOperationException("Invalid token representing a binary operation.");
            }
        }
    }
}

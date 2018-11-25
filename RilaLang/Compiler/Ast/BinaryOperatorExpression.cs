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
        /*
        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            switch(Operation)
            {
                case TokenType.Minus: return DLR.Expression.SubtractChecked(Lhs.GenerateExpressionTree(scope), Rhs.GenerateExpressionTree(scope));
                case TokenType.Plus: return DLR.Expression.AddChecked(Lhs.GenerateExpressionTree(scope), Rhs.GenerateExpressionTree(scope));
                case TokenType.Asterisk: return DLR.Expression.MultiplyChecked(Lhs.GenerateExpressionTree(scope), Rhs.GenerateExpressionTree(scope));
                case TokenType.Slash: return DLR.Expression.Divide(Lhs.GenerateExpressionTree(scope), Rhs.GenerateExpressionTree(scope));
                case TokenType.Modulo: return DLR.Expression.Modulo(Lhs.GenerateExpressionTree(scope), Rhs.GenerateExpressionTree(scope));
                case TokenType.And: return DLR.Expression.AndAlso(Lhs.GenerateExpressionTree(scope), Rhs.GenerateExpressionTree(scope));
                case TokenType.Or: return DLR.Expression.OrElse(Lhs.GenerateExpressionTree(scope), Rhs.GenerateExpressionTree(scope));
                case TokenType.GreaterThan: return DLR.Expression.GreaterThan(Lhs.GenerateExpressionTree(scope), Rhs.GenerateExpressionTree(scope));
                case TokenType.EqGreaterThan: return DLR.Expression.GreaterThanOrEqual(Lhs.GenerateExpressionTree(scope), Rhs.GenerateExpressionTree(scope));
                case TokenType.LessThan: return DLR.Expression.LessThan(Lhs.GenerateExpressionTree(scope), Rhs.GenerateExpressionTree(scope));
                case TokenType.EqLessThan: return DLR.Expression.LessThanOrEqual(Lhs.GenerateExpressionTree(scope), Rhs.GenerateExpressionTree(scope));
                case TokenType.Equal: return DLR.Expression.Equal(Lhs.GenerateExpressionTree(scope), Rhs.GenerateExpressionTree(scope));
                case TokenType.NotEqual: return DLR.Expression.NotEqual(Lhs.GenerateExpressionTree(scope), Rhs.GenerateExpressionTree(scope));
                default:
                    throw new InvalidOperationException("Invalid token representing a binary operation.");
                //case TokenType.Range: return;
            }
        }
        */

        public override DLR.Expression GenerateExpressionTree(GenScope scope) //TODO: Test
        {
            var lhs = Lhs.GenerateExpressionTree(scope);
            var rhs = Rhs.GenerateExpressionTree(scope);
            var op = GetOperation();
            //TODO: TokenType.Range
            return DLR.Expression.Dynamic(new RilaBinaryOperationBinder(op), typeof(object), lhs, rhs);
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

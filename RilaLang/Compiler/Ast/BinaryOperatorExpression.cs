using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Compiler.Ast
{
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
    }
}

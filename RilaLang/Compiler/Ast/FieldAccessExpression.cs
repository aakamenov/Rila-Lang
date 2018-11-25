using System.Linq.Expressions;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class FieldAccessExpression : Expression
    {
        public Expression Lhs { get; }
        public Expression Rhs { get; }

        public FieldAccessExpression(Expression lhs, Expression rhs)
        {
            Lhs = lhs;
            Rhs = rhs;
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            throw new System.NotImplementedException();
        }
    }
}

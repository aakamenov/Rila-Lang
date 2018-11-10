namespace RilaLang.Compiler.Ast
{
    public class FieldAccessExpression : Expression
    {
        public Expression Lhs { get; }
        public Expression Rhs { get; }

        public FieldAccessExpression(Expression lhs, Expression rhs)
        {
            Lhs = lhs;
            Rhs = rhs;
        }
    }
}

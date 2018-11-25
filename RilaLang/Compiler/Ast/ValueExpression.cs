namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public abstract class ValueExpression<T> : Expression
    {
        public T Value { get; }

        internal ValueExpression(T value)
        {
            Value = value;
        }
    }

    public sealed class NumberExpression : ValueExpression<int>
    {
        internal NumberExpression(int value) : base(value) { }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            return DLR.Expression.Constant(Value, typeof(int));
        }
    }

    public sealed class StringExpression : ValueExpression<string>
    {
        internal StringExpression(string value) : base(value) { }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            return DLR.Expression.Constant(Value, typeof(string));
        }
    }

    public sealed class BoolExpression : ValueExpression<bool>
    {
        internal BoolExpression(bool value) : base(value) { }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            return DLR.Expression.Constant(Value, typeof(bool));
        }
    }
}

namespace RilaLang.Compiler.Ast
{
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
    }

    public sealed class StringExpression : ValueExpression<string>
    {
        internal StringExpression(string value) : base(value) { }
    }

    public sealed class BoolExpression : ValueExpression<bool>
    {
        internal BoolExpression(bool value) : base(value) { }
    }
}

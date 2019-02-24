using RilaLang.Runtime.Binding;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class RangeExpression : Expression
    {
        public Expression Start { get; }
        public Expression End { get; }

        public RangeExpression(Expression start, Expression end)
        {
            Start = start;
            End = end;
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            var start = Start.GenerateExpressionTree(scope);
            var end = End.GenerateExpressionTree(scope);

            return DLR.Expression.Dynamic(new RangeOperationBinder(), typeof(object), start, end);
        }
    }
}

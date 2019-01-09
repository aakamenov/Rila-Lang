using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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

            return DLR.Expression.Dynamic(scope.Runtime.RangeOperationBinder, typeof(object), start, end);
        }
    }
}

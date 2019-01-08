using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class ReturnStatement : Statement
    {
        public Expression Expression { get; }

        public ReturnStatement(Expression expression)
        {
            Expression = expression;
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            if(Expression is null)
                return DLR.Expression.Goto(scope.BreakTarget);

            return DLR.Expression.Goto(scope.BreakTarget, Expression.GenerateExpressionTree(scope), typeof(object));
        }
    }
}

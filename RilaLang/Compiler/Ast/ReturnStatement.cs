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
            if (!scope.IsInLambda())
                throw new InvalidOperationException("No function to return from!");

            if(Expression is null)
                return DLR.Expression.Goto(scope.GetFirstLambdaScope().CreateReturnTarget());

            var expression = Expression.GenerateExpressionTree(scope);

            return DLR.Expression.Goto(scope.GetFirstLambdaScope().CreateReturnTarget(false), expression, expression.Type);
        }
    }
}

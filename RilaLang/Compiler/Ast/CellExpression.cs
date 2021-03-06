﻿using RilaLang.Runtime.Binding;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class CellExpression : Expression
    {
        public Expression Expression { get; }

        public CellExpression(Expression expression)
        {
            Expression = expression;
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            return DLR.Expression.Dynamic(
                new CreateCellInstanceBinder(), 
                typeof(object), 
                Expression.GenerateExpressionTree(scope));
        }
    }
}

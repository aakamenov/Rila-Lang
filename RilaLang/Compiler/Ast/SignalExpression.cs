using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RilaLang.Runtime.Binding;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class SignalExpression : Expression
    {
        public Expression Expression { get; }
        public IReadOnlyCollection<string> CellNames { get; }

        public SignalExpression(Expression expression, IList<string> cellNames)
        {
            Expression = expression;
            CellNames = new ReadOnlyCollection<string>(cellNames);
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            var args = new DLR.Expression[CellNames.Count + 1];
            args[0] = DLR.Expression.Quote(DLR.Expression.Lambda(Expression.GenerateExpressionTree(scope)));
            
            for (var i = 1; i < CellNames.Count + 1; i++)
            {
                var varName = CellNames.ElementAt(i - 1);

                if (scope.TryGetVariable(varName, out DLR.ParameterExpression variable))
                    args[i] = variable;
                else
                    throw new ArgumentException($"Variable \"{varName}\" is not defined.");
            }
            
            return DLR.Expression.Dynamic(
                new CreateSignalInstanceBinder(), 
                typeof(object),
                args);
        }
    }
}

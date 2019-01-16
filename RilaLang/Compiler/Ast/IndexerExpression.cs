using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RilaLang.Runtime.Binding;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class IndexerExpression : Expression
    {
        public Expression Identifier { get; }
        public IReadOnlyCollection<Expression> Parameters { get; }

        public IndexerExpression(Expression identifier, IList<Expression> parameters)
        {
            Identifier = identifier;
            Parameters = new ReadOnlyCollection<Expression>(parameters);
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            var args = new DLR.Expression[Parameters.Count + 1];
            args[0] = Identifier.GenerateExpressionTree(scope);

            for (int i = 1; i <= Parameters.Count; i++)
                args[i] = Parameters.ElementAt(i - 1).GenerateExpressionTree(scope);

            return DLR.Expression.Dynamic(new RilaGetIndexBinder(new CallInfo(Parameters.Count)), typeof(object), args);
        }
    }
}

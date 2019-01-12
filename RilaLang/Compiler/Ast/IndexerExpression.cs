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
            var args = new List<DLR.Expression>();
            args.Add(Identifier.GenerateExpressionTree(scope));
            args.AddRange(Parameters.Select(x => x.GenerateExpressionTree(scope)));

            return DLR.Expression.Dynamic(new RilaGetIndexBinder(new CallInfo(Parameters.Count)), typeof(object), args);
        }
    }
}

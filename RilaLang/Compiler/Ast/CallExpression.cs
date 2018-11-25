using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class CallExpression : Expression
    {
        public Expression Function { get; }
        public IReadOnlyCollection<Expression> Arguments { get; }

        public CallExpression(Expression function, IList<Expression> args)
        {
            Function = function;
            Arguments = new ReadOnlyCollection<Expression>(args);
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            throw new System.NotImplementedException();
        }
    }
}

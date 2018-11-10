using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RilaLang.Compiler.Ast
{
    public class CallExpression : Expression
    {
        public Expression Function { get; }
        public IReadOnlyCollection<Expression> Arguments { get; }

        public CallExpression(Expression function, IList<Expression> args)
        {
            Function = function;
            Arguments = new ReadOnlyCollection<Expression>(args);
        }
    }
}

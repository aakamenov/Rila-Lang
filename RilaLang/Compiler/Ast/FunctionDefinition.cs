using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class FunctionDefinition : Statement
    {
        public string Name { get; }
        public IReadOnlyCollection<Expression> Arguments { get; }
        public BlockExpression Body { get; }

        public FunctionDefinition(string name, IList<Expression> arguments, BlockExpression body)
        {
            Name = name;
            Arguments = new ReadOnlyCollection<Expression>(arguments);
            Body = body;
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            throw new NotImplementedException();
        }
    }
}

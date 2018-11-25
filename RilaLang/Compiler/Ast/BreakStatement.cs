using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class BreakStatement : Statement
    {
        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            var loop = scope.GetFirstLoopScope();

            if(loop is null)
                throw new InvalidOperationException("No enclosing loop to break out of.");

            return DLR.Expression.Break(loop.BreakTarget);
        }
    }
}

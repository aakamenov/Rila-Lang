using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public abstract class AstNode
    {
        public abstract DLR.Expression GenerateExpressionTree(GenScope scope);
    }
}

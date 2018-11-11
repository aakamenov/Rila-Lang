using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Compiler.Ast
{
    public class WhileLoopStatement : Statement
    {
        public Expression Condition { get; }
        public BlockExpression Block { get; }

        public WhileLoopStatement(Expression condition, BlockExpression block)
        {
            Condition = condition;
            Block = block;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Compiler.Ast
{
    public class ForLoopStatement : Statement
    {
        public string VariableName { get; }
        public Expression InExpression { get; }
        public BlockExpression Block { get; }

        public ForLoopStatement(string variableName, Expression inExpression, BlockExpression block)
        {
            VariableName = variableName;
            InExpression = inExpression;
            Block = block;
        }
    }
}

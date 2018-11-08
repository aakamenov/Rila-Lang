using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Compiler.Ast
{
    public class IfStatement : Statement
    {
        public class IfBranch
        {
            Expression Condition { get; }
            BlockExpression Block { get; }

            public IfBranch(Expression condition, BlockExpression block)
            {
                Condition = condition;
                Block = block;
            }
        }

        public IReadOnlyCollection<IfBranch> Branches { get; }
        public BlockExpression ElseBranch { get; }

        public IfStatement(IList<IfBranch> branches, BlockExpression elseBranch)
        {
            Branches = new ReadOnlyCollection<IfBranch>(branches);
            ElseBranch = elseBranch;
        }
    }
}

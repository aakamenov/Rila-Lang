using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class IfStatement : Statement
    {
        public class IfBranch
        {
            public Expression Condition { get; }
            public BlockExpression Block { get; }

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

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            var stmts = new DLR.ConditionalExpression[Branches.Count];
            var hasElse = ElseBranch != null;
            var count = hasElse ? Branches.Count - 1 : Branches.Count;

            for(int i = 0; i < count; i++)
            {
                var branch = Branches.ElementAt(i);
                var condition = branch.Condition.GenerateExpressionTree(scope);
                var block = branch.Block.GenerateExpressionTree(scope);

                stmts[i] = DLR.Expression.IfThen(condition, block);
            }

            if (hasElse)
            {
                var last = Branches.ElementAt(count);
                var condition = last.Condition.GenerateExpressionTree(scope);
                var block = last.Block.GenerateExpressionTree(scope);
                var elseBlock = ElseBranch.GenerateExpressionTree(scope);

                stmts[count] = DLR.Expression.IfThenElse(condition, block, elseBlock);
            }

            return DLR.Expression.Block(stmts);
        }
    }
}

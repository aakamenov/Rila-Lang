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
            var ifs = CreateConditionalExpression(scope, 0);

            return DLR.Expression.Block(ifs);
        }

        private DLR.ConditionalExpression CreateConditionalExpression(GenScope scope, int index)
        {
            var branch = Branches.ElementAt(index);

            var condition = DLR.Expression.Convert(branch.Condition.GenerateExpressionTree(scope), typeof(bool));
            var block = branch.Block.GenerateExpressionTree(scope);

            index++;

            if (index == Branches.Count)
            {
                if (ElseBranch is null)
                    return DLR.Expression.Condition(condition, block, DLR.Expression.Default(block.Type));
                else
                    return DLR.Expression.Condition(condition, block, ElseBranch.GenerateExpressionTree(scope), block.Type);
            }
            else
            {               
                return DLR.Expression.Condition(
                    condition,
                    block,
                    CreateConditionalExpression(scope, index));
            }
        }
    }
}

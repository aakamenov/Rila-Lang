using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Scripting.Ast;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class WhileLoopStatement : Statement
    {
        public Expression Condition { get; }
        public BlockExpression Block { get; }

        public WhileLoopStatement(Expression condition, BlockExpression block)
        {
            Condition = condition;
            Block = block;
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            var loop = GenScope.CreateLoop(scope);
            
            var condition = Condition.GenerateExpressionTree(scope);
            var block = Block.GenerateExpressionTree(loop);
            
            return Utils.While(condition, block, DLR.Expression.Empty(), scope.BreakTarget, scope.ContinueTarget);
        }
    }
}

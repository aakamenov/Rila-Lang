using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

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

        public override DLR.Expression GenerateExpressionTree(GenScope scope) //TODO: See IronPython 3 implementation
        {
            var loop = scope.CreateLoop();
            var bindVar = DLR.Expression.Parameter(typeof(object), VariableName);
            var inExpr = DLR.Expression.Dynamic(null, typeof(object));
            var block = Block.GenerateExpressionTree(loop);

            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class TryCatchStatement : Statement
    {
        public class CatchStatement
        {
            public Expression CatchExpression { get; }
            public BlockExpression CatchBlock { get; }
        }

        public BlockExpression TryBlock { get; }
        public IReadOnlyCollection<CatchStatement> CatchStatements { get; }
        public BlockExpression FinallyBlock { get; }

        public TryCatchStatement(BlockExpression tryBlock, IList<CatchStatement> catchStatements, BlockExpression finallyBlock)
        {
            TryBlock = tryBlock;
            CatchStatements = new ReadOnlyCollection<CatchStatement>(catchStatements);
            FinallyBlock = finallyBlock;
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            throw new NotImplementedException();
        }
    }
}

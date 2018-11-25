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

    public class BlockExpression : Expression
    {
        public IReadOnlyCollection<AstNode> Statements { get; }

        public BlockExpression(IList<AstNode> statements)
        {
            Statements = new ReadOnlyCollection<AstNode>(statements);
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            var stmts = new DLR.Expression[Statements.Count];

            for (int i = 0; i < Statements.Count; i++)
                stmts[i] = (Statements.ElementAt(i).GenerateExpressionTree(scope));

            return DLR.Expression.Block(stmts);
        }
    }
}

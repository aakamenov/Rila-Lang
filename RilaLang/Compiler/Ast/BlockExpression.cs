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
            var blockScope = scope.CreateChild();

            for (int i = 0; i < Statements.Count; i++)
                stmts[i] = Statements.ElementAt(i).GenerateExpressionTree(blockScope);

            return DLR.Expression.Block(blockScope.Definitions.Select(x => x.Value), stmts);
        }
    }
}

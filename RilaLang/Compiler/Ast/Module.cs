using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class Module : AstNode
    {
        public IReadOnlyCollection<AstNode> Statements { get; }
        public string Name { get; }

        public Module(string name, IList<AstNode> statements)
        {
            Statements = new ReadOnlyCollection<AstNode>(statements);
            Name = string.IsNullOrEmpty(name) ? Guid.NewGuid().ToString() : name;
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope = null)
        {
            scope = GenScope.CreateRoot();

            var stmts = new DLR.Expression[Statements.Count];

            for (int i = 0; i < Statements.Count; i++)
                stmts[i] = Statements.ElementAt(i).GenerateExpressionTree(scope);

            return DLR.Expression.Block(stmts);
        }
    }
}

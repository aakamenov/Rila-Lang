using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RilaLang.Runtime;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class Module
    {
        public IReadOnlyCollection<AstNode> Statements { get; }
        public string Name { get; }

        public Module(string name, IList<AstNode> statements)
        {
            Statements = new ReadOnlyCollection<AstNode>(statements);
            Name = string.IsNullOrEmpty(name) ? Guid.NewGuid().ToString() : name;
        }

        public DLR.Expression<Func<Rila, dynamic>> ConstructProgram(Rila runtime)
        {
            var scope = new GenScopeRoot(runtime);

            var stmts = new DLR.Expression[Statements.Count];

            for (int i = 0; i < Statements.Count; i++)
                stmts[i] = Statements.ElementAt(i).GenerateExpressionTree(scope);

            //Set the variables defined at the top level
            var globals = scope.Definitions.Select(x => x.Value);

            var block = DLR.Expression.Block(globals, stmts);

            if (block.Type == typeof(void)) //Program should always return a value
                block = DLR.Expression.Block(block, DLR.Expression.Default(typeof(object)));

            return DLR.Expression.Lambda<Func<Rila, dynamic>>(block, scope.Root.RuntimeParameter);
        }
    }
}

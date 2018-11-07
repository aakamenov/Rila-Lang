using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RilaLang.Compiler.Ast
{
    public class Module : AstNode
    {
        public IReadOnlyCollection<AstNode> Statements { get; }
        public string Name { get; }

        public Module(string name, IList<AstNode> statements)
        {
            Statements = new ReadOnlyCollection<AstNode>(statements);
            Name = string.IsNullOrEmpty(name) ? Guid.NewGuid().ToString() : name;
        }
    }
}

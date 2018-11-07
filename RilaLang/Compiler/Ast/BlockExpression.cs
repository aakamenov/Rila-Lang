using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Compiler.Ast
{
    public class BlockExpression : Expression
    {
        public IReadOnlyCollection<AstNode> Statements { get; }

        public BlockExpression(IList<AstNode> statements)
        {
            Statements = new ReadOnlyCollection<AstNode>(statements);
        }
    }
}

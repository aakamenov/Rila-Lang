using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Compiler.Ast
{
    internal class AssignmentStatement : AstNode, IStatement
    {
        public string Identifier { get; private set; }
        public IExpression Rhs { get; private set; }

        public AssignmentStatement(string identifier, IExpression expression)
        {
            Identifier = identifier;
            Rhs = expression;
        }
    }
}

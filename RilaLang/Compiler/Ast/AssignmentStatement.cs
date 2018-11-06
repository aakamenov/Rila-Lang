using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Compiler.Ast
{
    public class AssignmentStatement : Statement
    {
        public string Identifier { get; private set; }
        public Expression Rhs { get; private set; }

        public AssignmentStatement(string identifier, Expression expression)
        {
            Identifier = identifier;
            Rhs = expression;
        }
    }
}

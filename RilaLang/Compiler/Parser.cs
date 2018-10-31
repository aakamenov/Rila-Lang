using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RilaLang.Compiler.Ast;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RilaLang.Tests")]
namespace RilaLang.Compiler
{
    internal class Parser
    {
        private Lexer lexer;

        internal Parser(Lexer lexer)
        {
            this.lexer = lexer;
        }
    }
}

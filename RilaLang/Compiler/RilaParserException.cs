using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Compiler
{
    public class RilaParserException : Exception
    {
        internal RilaParserException(string message) : base(message) { }
        internal RilaParserException(string message, Exception inner) : base(message, inner) { }
    }
}

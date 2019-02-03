using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Runtime
{
    public class RilaRuntimeException : Exception
    {
        internal RilaRuntimeException(string message) : base(message) { }
        internal RilaRuntimeException(string message, Exception inner) : base(message, inner) { }
    }
}

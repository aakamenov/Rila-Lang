using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Runtime
{
    public class RilaRuntimeException : Exception
    {
        public RilaRuntimeException(string message) : base(message) { }
        public RilaRuntimeException(string message, Exception inner) : base(message, inner) { }
    }
}

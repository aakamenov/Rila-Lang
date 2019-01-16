using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Dynamic;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using RilaLang.Runtime;
using RilaLang.Runtime.Binding;

namespace RilaLang.Hosting
{
    public class RilaScriptCode : ScriptCode
    {
        private readonly Func<Rila, dynamic> compiledCode;
        private readonly Rila runtime;

        public RilaScriptCode(Func<Rila, dynamic> compiledCode, Rila runtime, SourceUnit sourceUnit) : base(sourceUnit)
        {
            this.compiledCode = compiledCode;
            this.runtime = runtime;
        }

        public override object Run()
        {
            return Run(new Scope());
        }

        public override object Run(Scope scope)
        {
            return compiledCode(runtime);
        }
    }
}

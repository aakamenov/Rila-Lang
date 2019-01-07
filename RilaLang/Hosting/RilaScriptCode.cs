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
        private readonly Func<dynamic> compiledCode;

        public RilaScriptCode(Func<dynamic> compiledCode, SourceUnit sourceUnit) : base(sourceUnit)
        {
            this.compiledCode = compiledCode;
        }

        public override object Run()
        {
            return Run(new Scope());
        }

        public override object Run(Scope scope)
        {
            return compiledCode();
        }
    }
}

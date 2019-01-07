using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Linq.Expressions;
using Microsoft.Scripting;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Utils;
using RilaLang.Runtime;
using RilaLang.Compiler;

namespace RilaLang.Hosting
{
    public class RilaContext : LanguageContext
    {
        private readonly Rila rila;

        // The DLR uses reflection to instantiate this and requires the "options" param
        public RilaContext(ScriptDomainManager domainManager, IDictionary<string, object> options) : base(domainManager)
        {
            rila = new Rila(domainManager.GetLoadedAssemblyList(), domainManager.Globals);
        }
        
        public override ScriptCode CompileSourceCode(SourceUnit sourceUnit, CompilerOptions options, ErrorSink errorSink)
        {
            try
            {
                var lexer = new Lexer(sourceUnit.GetReader().ReadToEnd());
                var program = new RilaParser(lexer).Parse().ConstructProgram(rila);
                               
                return new RilaScriptCode(program.Compile(), sourceUnit);
            }
            catch(Exception e)
            {
                errorSink.Add(sourceUnit, e.Message, SourceSpan.None, 0, Severity.FatalError);
            }

            return null;
        }

        public override TService GetService<TService>(params object[] args)
        {
            if (typeof(TService) == typeof(Rila))
                return (TService)(object)rila;

            return base.GetService<TService>(args);
        }
    }
}

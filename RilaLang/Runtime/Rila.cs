using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Dynamic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Scripting.Runtime;
using Microsoft.Scripting.Hosting;
using RilaLang.Runtime.Binding;
using RilaLang.Runtime.Binding.Utils;
using RilaLang.Hosting;

namespace RilaLang.Runtime
{
    public class Rila
    {
        public const string LANG_NAME = "Rila";

        public Scope DlrGlobals { get; }
        public ExpandoObject Globals { get; }

        private Dictionary<ExpressionType, RilaBinaryOperationBinder> binaryOperationBinders;
        private IList<Assembly> assemblies;

        public Rila(IList<Assembly> assemblies, Scope dlrGlobals)
        {
            DlrGlobals = dlrGlobals;
            Globals = new ExpandoObject();

            this.assemblies = assemblies;
            binaryOperationBinders = new Dictionary<ExpressionType, RilaBinaryOperationBinder>();

            //AddAssemblyNamesAndTypes();
        }
        
        public static ScriptEngine CreateRilaEngine()
        {
            return new ScriptRuntime(CreateRuntimeSetup()).GetEngine(LANG_NAME);
        }

        public static ScriptRuntimeSetup CreateRuntimeSetup()
        {
            var asmName = typeof(RilaContext).AssemblyQualifiedName;
            var setup = new ScriptRuntimeSetup();
            setup.LanguageSetups.Add(new LanguageSetup(asmName, "Rila", new[] { LANG_NAME }, new[] { ".rila" }));

            return setup;
        }

        public RilaBinaryOperationBinder GetBinaryOperationBinder(ExpressionType operation)
        {
            if (binaryOperationBinders.TryGetValue(operation, out RilaBinaryOperationBinder result))
                return result;

            var binder = new RilaBinaryOperationBinder(operation);
            binaryOperationBinders[operation] = binder;

            return binder;
        }
    }
}

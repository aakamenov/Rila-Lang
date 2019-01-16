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

        public RangeOperationBinder RangeOperationBinder { get; }

        public TypeProvider TypeProvider { get; }

        private Dictionary<ExpressionType, RilaBinaryOperationBinder> binaryOperationBinders;
        private Dictionary<string, RilaGetMemberBinder> getMemberBinders;
        private Dictionary<CallInfo, RilaCreateInstanceBinder> createInstanceBinders;

        public Rila(IList<Assembly> assemblies, Scope dlrGlobals)
        {
            DlrGlobals = dlrGlobals;
            Globals = new ExpandoObject();

            TypeProvider = new TypeProvider(assemblies);

            binaryOperationBinders = new Dictionary<ExpressionType, RilaBinaryOperationBinder>();
            getMemberBinders = new Dictionary<string, RilaGetMemberBinder>();
            createInstanceBinders = new Dictionary<CallInfo, RilaCreateInstanceBinder>();
            RangeOperationBinder = new RangeOperationBinder();
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
            if (binaryOperationBinders.TryGetValue(operation, out RilaBinaryOperationBinder binder))
                return binder;

            binder = new RilaBinaryOperationBinder(operation);
            binaryOperationBinders[operation] = binder;

            return binder;
        }

        public RilaGetMemberBinder GetGetMemberBinder(string memberName)
        {
            if (getMemberBinders.TryGetValue(memberName, out RilaGetMemberBinder binder))
                return binder;

            binder = new RilaGetMemberBinder(memberName, this);
            getMemberBinders[memberName] = binder;

            return binder;
        }

        public RilaCreateInstanceBinder GetCreateInstanceBinder(CallInfo callInfo)
        {
            if (createInstanceBinders.TryGetValue(callInfo, out RilaCreateInstanceBinder binder))
                return binder;

            binder = new RilaCreateInstanceBinder(callInfo, this);
            createInstanceBinders[callInfo] = binder;

            return binder;
        }
    }
}

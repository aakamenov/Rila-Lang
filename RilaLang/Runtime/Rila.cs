using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Scripting.Runtime;
using RilaLang.Runtime.Binding;

namespace RilaLang.Runtime
{
    public class Rila
    {
        private Dictionary<ExpressionType, RilaBinaryOperationBinder> binaryOperationBinders;

        public Rila()
        {
            binaryOperationBinders = new Dictionary<ExpressionType, RilaBinaryOperationBinder>();
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

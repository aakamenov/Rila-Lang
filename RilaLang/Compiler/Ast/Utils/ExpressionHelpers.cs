using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RilaLang.Runtime;
using RilaLang.Runtime.Binding.Utils;

namespace RilaLang.Compiler.Ast.Utils
{
    using DLR = System.Linq.Expressions;

    public static class ExpressionHelpers
    {
        public static DLR.Expression ConstructGetTypeExpression(GenScope scope, string typeName)
        {
            var getTypeProvider = DLR.Expression.Dynamic(
                scope.Runtime.GetGetMemberBinder(nameof(Rila.TypeProvider)),
                typeof(object),
                scope.Root.RuntimeParameter);

            return DLR.Expression.Dynamic(
                scope.Runtime.GetInvokeMemberBinder(new Tuple<string, CallInfo>(nameof(TypeProvider.GetType), new CallInfo(1))),
                typeof(object),
                getTypeProvider,
                DLR.Expression.Constant(new UnresolvedType(typeName)));
        }
    }
}

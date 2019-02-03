using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RilaLang.Runtime;
using RilaLang.Runtime.Binding.Utils;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class TypeOfExpression : Expression
    {
        public string TypeName { get; }

        public TypeOfExpression(string typeName)
        {
            TypeName = typeName;
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            var getTypeProvider = DLR.Expression.Dynamic(
                scope.Runtime.GetGetMemberBinder(nameof(Rila.TypeProvider)),
                typeof(object),
                scope.Root.RuntimeParameter);

            return DLR.Expression.Dynamic(
                scope.Runtime.GetInvokeMemberBinder(new Tuple<string, CallInfo>(nameof(TypeProvider.GetType), new CallInfo(1))),
                typeof(object),
                getTypeProvider,
                DLR.Expression.Constant(new UnresolvedType(TypeName)));
        }
    }
}

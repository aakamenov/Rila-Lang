using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RilaLang.Runtime;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class UseStatement : Statement
    {
        public string Namespace { get; }
        public string Alias { get; }

        public UseStatement(string @namespace, string alias)
        {
            Namespace = @namespace;
            Alias = alias;
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            var getTypeProvider = DLR.Expression.Dynamic(
                scope.Runtime.GetGetMemberBinder(nameof(Rila.TypeProvider)),
                typeof(object),
                scope.Root.RuntimeParameter);    
            
            return DLR.Expression.Dynamic(
                scope.Runtime.GetInvokeMemberBinder(new Tuple<string, CallInfo>(nameof(TypeProvider.LoadNamespace), new CallInfo(2))),
                typeof(object),
                getTypeProvider,
                DLR.Expression.Constant(Namespace),
                DLR.Expression.Constant(Alias));
        }
    }
}

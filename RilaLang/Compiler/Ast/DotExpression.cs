using System;
using System.Dynamic;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using RilaLang.Runtime.Binding;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class DotExpression : Expression
    {
        public IReadOnlyCollection<Expression> Expressions { get; }

        public DotExpression(IList<Expression> expression)
        {
            Expressions = new ReadOnlyCollection<Expression>(expression);
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            DLR.Expression result = Expressions.First().GenerateExpressionTree(scope);

            for(var element = 1; element < Expressions.Count; element++)
            {
                switch (Expressions.ElementAt(element))
                {
                    case IdentifierExpression identifier:
                        result = DLR.Expression.Dynamic(scope.Runtime.GetGetMemberBinder(identifier.Name), typeof(object), result);
                        break;
                    case CallExpression call:
                        {
                            var name = call.Function as IdentifierExpression;
                            var args = new DLR.Expression[call.Arguments.Count + 1];
                            args[0] = result;

                            for (var i = 1; i <= call.Arguments.Count; i++)
                                args[i] = call.Arguments.ElementAt(i - 1).GenerateExpressionTree(scope);

                            result = DLR.Expression.Dynamic(
                                new RilaInvokeMemberBinder(name.Name, new CallInfo(call.Arguments.Count)),
                                typeof(object),
                                args);
                        }
                        break;
                    case IndexerExpression indexer:
                        throw new NotImplementedException(); //TODO: resolve indexing
                }
            }

            return result;
        }
    }
}

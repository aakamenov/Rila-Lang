using System;
using System.Dynamic;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using RilaLang.Compiler.Ast.Utils;
using RilaLang.Runtime.Binding;
using RilaLang.Runtime.Binding.Utils;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class DotExpression : Expression
    {
        public IReadOnlyCollection<Expression> Expressions { get; }
        public bool IsSetMember { get; }

        public DotExpression(IList<Expression> expression, bool isSetMember)
        {
            Expressions = new ReadOnlyCollection<Expression>(expression);
            IsSetMember = isSetMember;
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            DLR.Expression result = null;

            var first = Expressions.First();
            
            if (first is IdentifierExpression ident)
            {
                if (scope.TryGetVariable(ident.Name, out ParameterExpression variable)) //expression performed on a variable
                    result = variable;
                else
                    result = DLR.Expression.Constant(new UnresolvedType(ident.Name)); //static or alias access
            }
            else
                result = first.GenerateExpressionTree(scope);
            
            for(var element = 1; element < Expressions.Count; element++)
            {
                switch (Expressions.ElementAt(element))
                {
                    case IdentifierExpression identifier:
                        {
                            if(IsSetMember && element == Expressions.Count - 2)
                            {
                                result = DLR.Expression.Dynamic(
                                    new RilaSetMemberBinder(identifier.Name),
                                    typeof(object),
                                    result,
                                    Expressions.Last().GenerateExpressionTree(scope)); //The last expression is the new value

                                element = Expressions.Count;
                            }
                            else
                                result = DLR.Expression.Dynamic(scope.Runtime.GetGetMemberBinder(identifier.Name), typeof(object), result);
                        }
                        break;
                    case CallExpression call:
                        {
                            var name = call.Function as IdentifierExpression;
                            var args = new DLR.Expression[call.Arguments.Count + 1];
                            args[0] = result;

                            for (var i = 1; i <= call.Arguments.Count; i++)
                            {
                                var arg = call.Arguments.ElementAt(i - 1);

                                if(arg is IdentifierExpression identifier) //Argument could be a type itself
                                {
                                    if(!scope.TryGetVariable(identifier.Name, out ParameterExpression _))
                                    {
                                        args[i] = ExpressionHelpers.ConstructGetTypeExpression(scope, identifier.Name);
                                        continue;
                                    }
                                }

                                args[i] = arg.GenerateExpressionTree(scope);
                            }

                            result = DLR.Expression.Dynamic(
                                scope.Runtime.GetInvokeMemberBinder(new Tuple<string, CallInfo>(name.Name, new CallInfo(call.Arguments.Count))),
                                typeof(object),
                                args);
                        }
                        break;
                    case IndexerExpression indexer:
                        result = indexer.GenerateExpressionTree(scope);
                        break;
                    default:
                        throw new ArgumentException("Expecting member access, call or indexer expression!");
                }
            }

            return result;
        }
    }
}

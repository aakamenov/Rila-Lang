using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using RilaLang.Runtime.Binding;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class ForLoopStatement : Statement
    {
        public string VariableName { get; }
        public Expression InExpression { get; }
        public BlockExpression Block { get; }

        public ForLoopStatement(string variableName, Expression inExpression, BlockExpression block)
        {
            VariableName = variableName;
            InExpression = inExpression;
            Block = block;
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            var loopScope = scope.CreateLoop();

            var bindVar = DLR.Expression.Variable(typeof(object), VariableName);
            var inExpr = InExpression.GenerateExpressionTree(scope);

            loopScope.Definitions[VariableName] = bindVar;
            var block = Block.GenerateExpressionTree(loopScope);

            var enumeratorVar = DLR.Expression.Variable(typeof(object), "$enumeratorVar");

            var getEnumeratorCall = DLR.Expression.Dynamic(
                scope.Runtime.GetInvokeMemberBinder(new Tuple<string, CallInfo>("GetEnumerator", new CallInfo(0))),
                typeof(object), 
                inExpr);

            var enumeratorAssign = DLR.Expression.Assign(enumeratorVar, getEnumeratorCall);
            var enumeratorDispose = DLR.Expression.Dynamic(scope.Runtime.GetInvokeMemberBinder(new Tuple<string, CallInfo>("Dispose", new CallInfo(0))),
                typeof(void),
                enumeratorVar);

            var moveNextCall = DLR.Expression.Dynamic(scope.Runtime.GetInvokeMemberBinder(new Tuple<string, CallInfo>("MoveNext", new CallInfo(0))),
                typeof(object),
                enumeratorVar);

            var breakLabel = DLR.Expression.Label("ForLoopBreak");

            var loop =
                DLR.Expression.Loop(
                    DLR.Expression.IfThenElse(
                        DLR.Expression.Convert(
                            DLR.Expression.Dynamic(scope.Runtime.GetBinaryOperationBinder(ExpressionType.Equal), typeof(object), moveNextCall, DLR.Expression.Constant(true)), 
                        typeof(bool)),
                        DLR.Expression.Block(
                            new[] { bindVar },
                            DLR.Expression.Assign(bindVar, DLR.Expression.Dynamic(scope.Runtime.GetGetMemberBinder("Current"), typeof(object), enumeratorVar)),
                            block),
                        DLR.Expression.Break(breakLabel)),
                    breakLabel);

            var tryFinally =
                DLR.Expression.TryFinally(
                    loop,
                    DLR.Expression.IfThen( //Arrays do not implement IDisposable, so we need to check
                        DLR.Expression.TypeIs(enumeratorVar, typeof(IDisposable)), enumeratorDispose));

            var body =
                DLR.Expression.Block(
                    new[] { enumeratorVar },
                    enumeratorAssign,
                    tryFinally);

            return body;
        }
    }
}

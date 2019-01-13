using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RilaLang.Compiler.Ast;

namespace RilaLang.Compiler.Parselets
{
    public class FieldAccessParselet : IInfixParselet
    {
        public Precedence Precedence => Precedence.Postfix;

        public Expression Parse(RilaParser parser, Token token, Expression lhs)
        {
            const string error = "Expecting a call, identifier or a literal value";

            if(!CheckType(lhs))
                parser.AppendError(error, token);

            var rhs = parser.ParseExpression();

            if(!CheckType(rhs))
                parser.AppendError(error, parser.Peek());

            return new FieldAccessExpression(lhs, rhs);
        }

        private bool CheckType(Expression expression) //TODO: maybe remove this?
        {
            if (expression is CallExpression ||
                expression is IdentifierExpression ||
                expression is FieldAccessExpression ||
                expression is IndexerExpression ||
                expression.GetType().IsAssignableFrom(typeof(ValueExpression<>)))
            {
                return true;
            }

            return false;
        }
    }
}

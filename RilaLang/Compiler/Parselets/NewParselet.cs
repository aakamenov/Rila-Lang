using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RilaLang.Compiler.Ast;

namespace RilaLang.Compiler.Parselets
{
    public class NewParselet : IPrefixParselet
    {
        public Expression Parse(RilaParser parser, Token token)
        {
            var args = new List<Expression>();

            var expression = parser.ParseExpression();

            if(expression is DotExpression)
            {
                var dot = expression as DotExpression;

                var call = dot.Expressions.Last() as CallExpression;
                var ctor = call.Function as IdentifierExpression;

                if (call is null)
                    parser.AppendError("Expecting a constructor call!", token);

                var namespaces = dot.Expressions.ToList().GetRange(0, dot.Expressions.Count - 1).Select(x => x as IdentifierExpression).ToList();
                namespaces.Add(ctor);

                if (namespaces.Any(x => x is null))
                    parser.AppendError("Expecting namespace definition!", token);

                return new NewExpression(string.Join(".", namespaces.Select(x => x.Name)), call.Arguments);
            }
            else if(expression is CallExpression)
            {
                var call = expression as CallExpression;
                var identifier = call.Function as IdentifierExpression;

                if (identifier is null)
                    parser.AppendError("Expecting a constructor call!", token);

                return new NewExpression(identifier.Name, call.Arguments);
            }

            return null;
        }
    }
}

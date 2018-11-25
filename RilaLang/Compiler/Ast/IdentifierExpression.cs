using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Compiler.Ast
{
    using DLR = System.Linq.Expressions;

    public class IdentifierExpression : Expression
    {
        public string Name { get; }

        public IdentifierExpression(string name)
        {
            Name = name;
        }

        public override DLR.Expression GenerateExpressionTree(GenScope scope)
        {
            var param = DLR.Expression.Parameter(typeof(object), Name);
            scope.Definitions.Add(Name, param);

            return param;
        }
    }
}

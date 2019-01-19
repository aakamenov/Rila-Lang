using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RilaLang.Runtime.Binding.Utils;

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
            if(scope.TryGetVariable(Name, out DLR.ParameterExpression result))
            {
                return result;
            }  

            throw new InvalidOperationException($"\"{Name}\" is not defined!");
        }
    }
}

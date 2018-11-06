﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Compiler.Ast
{
    public class PostfixOperatorExpression : Expression
    {
        public Expression Lhs { get; }
        public TokenType Operation { get; }

        public PostfixOperatorExpression(Expression lhs, TokenType operation)
        {
            Lhs = lhs;
            Operation = operation;
        }
    }
}

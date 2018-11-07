﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RilaLang.Compiler.Ast;

namespace RilaLang.Compiler.Parselets
{
    public class IdentifierParselet : IPrefixParselet
    {
        public Expression Parse(RilaParser parser, Token token)
        {
            return new IdentifierExpression(token.Content);
        }
    }
}
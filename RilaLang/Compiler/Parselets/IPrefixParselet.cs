using System;
using System.Collections.Generic;
using System.Linq;
using RilaLang.Compiler.Ast;

namespace RilaLang.Compiler.Parselets
{
    public interface IPrefixParselet
    {
        Expression Parse(RilaParser parser, Token token);
    }
}

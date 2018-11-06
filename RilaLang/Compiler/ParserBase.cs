using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RilaLang.Compiler.Parselets;

namespace RilaLang.Compiler
{
    public abstract class ParserBase
    {
        protected Dictionary<TokenType, IPrefixParselet> prefixParselets;
        protected Dictionary<TokenType, IInfixParselet> infixParselets;

        internal ParserBase()
        {
            prefixParselets = new Dictionary<TokenType, IPrefixParselet>();
            infixParselets = new Dictionary<TokenType, IInfixParselet>();

            RegisterPrefix(TokenType.Minus, new PrefixOperatorParslet());
            RegisterPrefix(TokenType.Plus, new PrefixOperatorParslet());
            RegisterPrefix(TokenType.Bang, new PrefixOperatorParslet());
        }

        private void RegisterPrefix(TokenType type, IPrefixParselet handler)
        {
            prefixParselets.Add(type, handler);
        }
    }
}

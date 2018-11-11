using System;
using System.Collections.Generic;
using RilaLang.Compiler.Parselets;

namespace RilaLang.Compiler
{
    public partial class RilaParser
    {
        private Dictionary<TokenType, IPrefixParselet> prefixParselets;
        private Dictionary<TokenType, IInfixParselet> infixParselets;

        private void ConfigureExpressionRules()
        {
            prefixParselets = new Dictionary<TokenType, IPrefixParselet>();
            infixParselets = new Dictionary<TokenType, IInfixParselet>();

            RegisterPrefix(TokenType.Identifier, new IdentifierParselet());
            RegisterPrefix(TokenType.LParen, new GroupParselet());
            RegisterInfix(TokenType.LParen, new CallParselet());
            RegisterPrefix(TokenType.True, new BoolParselet());
            RegisterPrefix(TokenType.False, new BoolParselet());
            RegisterPrefix(TokenType.StringLiteral, new StringParselet());
            RegisterPrefix(TokenType.NumericLiteral, new NumberParselet());

            //Prefix operators
            RegisterPrefix(TokenType.Minus, new PrefixOperatorParslet(Precedence.Prefix));
            RegisterPrefix(TokenType.Plus, new PrefixOperatorParslet(Precedence.Prefix));
            RegisterPrefix(TokenType.Not, new PrefixOperatorParslet(Precedence.Not));

            //Math
            RegisterInfix(TokenType.Minus, new BinaryOperatorParslet(Precedence.Sum));
            RegisterInfix(TokenType.Plus, new BinaryOperatorParslet(Precedence.Sum));
            RegisterInfix(TokenType.Asterisk, new BinaryOperatorParslet(Precedence.Product));
            RegisterInfix(TokenType.Slash, new BinaryOperatorParslet(Precedence.Product));
            RegisterInfix(TokenType.Modulo, new BinaryOperatorParslet(Precedence.Product));

            //Booleans
            RegisterInfix(TokenType.And, new BinaryOperatorParslet(Precedence.And));
            RegisterInfix(TokenType.Or, new BinaryOperatorParslet(Precedence.Or));
            RegisterInfix(TokenType.GreaterThan, new BinaryOperatorParslet(Precedence.Equality));
            RegisterInfix(TokenType.LessThan, new BinaryOperatorParslet(Precedence.Equality));
            RegisterInfix(TokenType.EqGreaterThan, new BinaryOperatorParslet(Precedence.Equality));
            RegisterInfix(TokenType.EqLessThan, new BinaryOperatorParslet(Precedence.Equality));
            RegisterInfix(TokenType.Equal, new BinaryOperatorParslet(Precedence.Equality));
            RegisterInfix(TokenType.NotEqual, new BinaryOperatorParslet(Precedence.Equality));

            //Other
            RegisterInfix(TokenType.Dot, new FieldAccessParselet());
            RegisterInfix(TokenType.Range, new BinaryOperatorParslet(Precedence.Postfix));
        }

        private void RegisterPrefix(TokenType type, IPrefixParselet handler)
        {
            prefixParselets.Add(type, handler);
        }

        private void RegisterInfix(TokenType type, IInfixParselet handler)
        {
            infixParselets.Add(type, handler);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RilaLang.Compiler.Ast;

namespace RilaLang.Compiler.Parselets
{
    public class DotExpressionParselet : IInfixParselet
    {
        public Precedence Precedence => Precedence.Postfix;

        public Expression Parse(RilaParser parser, Token token, Expression lhs)
        {
            var expressions = new List<Expression>() { lhs };

            do
            {
                expressions.Add(parser.ParseExpression(Precedence));
            }
            while (parser.ConsumeIf(TokenType.Dot));

            return new DotExpression(expressions);
        }

        private bool CheckType(Expression expression) //TODO: maybe remove this?
        {
            if (expression is CallExpression ||
                expression is IdentifierExpression ||
                expression is IndexerExpression ||
                expression.GetType().IsAssignableFrom(typeof(ValueExpression<>)))
            {
                return true;
            }

            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Xunit;
using Xunit.Sdk;
using RilaLang.Compiler;
using RilaLang.Compiler.Ast;
using RilaLang.Compiler.Parselets;

namespace RilaLang.Tests
{
    public class ParserTests
    {
        [Fact]
        public void ParseAssignmentStatement()
        {

        }

        [Fact]
        public void ParseIfStatement()
        {

        }

        [Fact]
        public void ParseEqualityExpression()
        {
            var expression = "not a and (b >= c)";
            var lexer = new Lexer(expression);
            var parser = new RilaParser(lexer);

            var ast = parser.ParseExpression();
            var a = 0;
        }

        [Fact]
        public void ParseMultAndPlusExpression()
        {
            var expression = "a * b + c";
            var lexer = new Lexer(expression);
            var parser = new RilaParser(lexer);

            var ast = parser.ParseExpression();
            Assert.True(ast is BinaryOperatorExpression);

            var op = ast as BinaryOperatorExpression;
            Assert.True(op.Operation == TokenType.Plus);

            Assert.True(op.Lhs is BinaryOperatorExpression);
            var lhs = op.Lhs as BinaryOperatorExpression;

            Assert.True(lhs.Operation == TokenType.Asterisk);
            Assert.True(lhs.Lhs is IdentifierExpression);
            Assert.True((lhs.Lhs as IdentifierExpression).Name == "a");

            Assert.True(lhs.Rhs is IdentifierExpression);
            Assert.True((lhs.Rhs as IdentifierExpression).Name == "b");

            Assert.True(op.Rhs is IdentifierExpression);
            Assert.True((op.Rhs as IdentifierExpression).Name == "c");
        }

        [Fact]
        public void ParseMultAndPlusPrecedenceExpression()
        {
            var expression = "a * (b + c)";
            var lexer = new Lexer(expression);
            var parser = new RilaParser(lexer);

            var ast = parser.ParseExpression();
            Assert.True(ast is BinaryOperatorExpression);

            var op = ast as BinaryOperatorExpression;
            Assert.True(op.Operation == TokenType.Asterisk);

            Assert.True(op.Rhs is BinaryOperatorExpression);
            var rhs = op.Rhs as BinaryOperatorExpression;

            Assert.True(rhs.Operation == TokenType.Plus);
            Assert.True(rhs.Lhs is IdentifierExpression);
            Assert.True((rhs.Lhs as IdentifierExpression).Name == "b");

            Assert.True(rhs.Rhs is IdentifierExpression);
            Assert.True((rhs.Rhs as IdentifierExpression).Name == "c");

            Assert.True(op.Lhs is IdentifierExpression);
            Assert.True((op.Lhs as IdentifierExpression).Name == "a");
        }
    }
}

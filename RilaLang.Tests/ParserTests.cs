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

namespace RilaLang.Tests
{
    public class ParserTests
    {
        [Fact]
        public void ParseAssignmentStatement()
        {
            var statement = "a = a + 1";
            var lexer = new Lexer(statement);
            var parser = new RilaParser(lexer);

            var ast = parser.Parse() as Module;
            Assert.True(ast.Statements.Count == 1);
            var first = ast.Statements.First();

            Assert.IsType<AssignmentStatement>(first);
            var assign = first as AssignmentStatement;
            Assert.True(assign.Identifier == "a");

            Assert.IsType<BinaryOperatorExpression>(assign.Rhs);
            var plus = assign.Rhs as BinaryOperatorExpression;
            Assert.True(plus.Operation == TokenType.Plus);

            Assert.IsType<IdentifierExpression>(plus.Lhs);
            Assert.True((plus.Lhs as IdentifierExpression).Name == "a");

            Assert.IsType<NumberExpression>(plus.Rhs);
            Assert.True((plus.Rhs as NumberExpression).Value == 1);
        }

        [Fact]
        public void ParseIfStatements()
        {
            var program = File.ReadAllText("TestPrograms/Parser/if.rila");
            var lexer = new Lexer(program);
            var parser = new RilaParser(lexer);

            var ast = parser.Parse();
        }

        [Fact]
        public void ParseEqualityExpression()
        {
            var expression = "not a and b >= c";
            var lexer = new Lexer(expression);
            var parser = new RilaParser(lexer);

            var ast = parser.ParseExpression();
            Assert.IsType<BinaryOperatorExpression>(ast);
            var and = ast as BinaryOperatorExpression;
            Assert.True(and.Operation == TokenType.And);

            Assert.IsType<PrefixOperatorExpression>(and.Lhs);
            var not = and.Lhs as PrefixOperatorExpression;
            Assert.True(not.Operation == TokenType.Not);
            Assert.IsType<IdentifierExpression>(not.Rhs);
            var a = not.Rhs as IdentifierExpression;
            Assert.True(a.Name == "a");

            Assert.IsType<BinaryOperatorExpression>(and.Rhs);
            var eqGt = and.Rhs as BinaryOperatorExpression;
            Assert.True(eqGt.Operation == TokenType.EqGreaterThan);
            Assert.IsType<IdentifierExpression>(eqGt.Lhs);
            var b = eqGt.Lhs as IdentifierExpression;
            Assert.True(b.Name == "b");
            Assert.IsType<IdentifierExpression>(eqGt.Rhs);
            var c = eqGt.Rhs as IdentifierExpression;
            Assert.True(c.Name == "c");
        }

        [Fact]
        public void ParseMultAndPlusExpression()
        {
            var expression = "a * b + c";
            var lexer = new Lexer(expression);
            var parser = new RilaParser(lexer);

            var ast = parser.ParseExpression();
            Assert.IsType<BinaryOperatorExpression>(ast);

            var op = ast as BinaryOperatorExpression;
            Assert.True(op.Operation == TokenType.Plus);

            Assert.IsType<BinaryOperatorExpression>(op.Lhs);
            var lhs = op.Lhs as BinaryOperatorExpression;

            Assert.True(lhs.Operation == TokenType.Asterisk);
            Assert.IsType<IdentifierExpression>(lhs.Lhs);
            Assert.True((lhs.Lhs as IdentifierExpression).Name == "a");

            Assert.IsType<IdentifierExpression>(lhs.Rhs);
            Assert.True((lhs.Rhs as IdentifierExpression).Name == "b");

            Assert.IsType<IdentifierExpression>(op.Rhs);
            Assert.True((op.Rhs as IdentifierExpression).Name == "c");
        }

        [Fact]
        public void ParseMultAndPlusPrecedenceExpression()
        {
            var expression = "a * (b + c)";
            var lexer = new Lexer(expression);
            var parser = new RilaParser(lexer);

            var ast = parser.ParseExpression();
            Assert.IsType<BinaryOperatorExpression>(ast);

            var op = ast as BinaryOperatorExpression;
            Assert.True(op.Operation == TokenType.Asterisk);

            Assert.IsType<BinaryOperatorExpression>(op.Rhs);
            var rhs = op.Rhs as BinaryOperatorExpression;

            Assert.True(rhs.Operation == TokenType.Plus);
            Assert.IsType<IdentifierExpression>(rhs.Lhs);
            Assert.True((rhs.Lhs as IdentifierExpression).Name == "b");

            Assert.IsType<IdentifierExpression>(rhs.Rhs);
            Assert.True((rhs.Rhs as IdentifierExpression).Name == "c");

            Assert.IsType<IdentifierExpression>(op.Lhs);
            Assert.True((op.Lhs as IdentifierExpression).Name == "a");
        }
    }
}

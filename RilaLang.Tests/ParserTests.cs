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
            Assert.True(ast.Statements.Count == 1);
            Assert.IsType<IfStatement>(ast.Statements.First());

            var ifStmt = ast.Statements.First() as IfStatement;
            Assert.True(ifStmt.Branches.Count == 3);

            var first = ifStmt.Branches.First();
            Assert.IsType<BinaryOperatorExpression>(first.Condition);
            Assert.True(first.Block.Statements.Count == 3);
            Assert.IsType<AssignmentStatement>(first.Block.Statements.First());
            Assert.IsType<AssignmentStatement>(first.Block.Statements.ElementAt(1));
            Assert.IsType<IfStatement>(first.Block.Statements.ElementAt(2));

            var firstNestedIf = first.Block.Statements.ElementAt(2) as IfStatement;
            Assert.True(firstNestedIf.Branches.Count == 1);
            Assert.IsType<BinaryOperatorExpression>(firstNestedIf.Branches.First().Condition);
            Assert.IsType<AssignmentStatement>(firstNestedIf.Branches.First().Block.Statements.First());

            var second = ifStmt.Branches.ElementAt(1);
            Assert.IsType<BoolExpression>(second.Condition);
            Assert.True(second.Block.Statements.Count == 1);
            Assert.IsType<AssignmentStatement>(second.Block.Statements.First());

            var third = ifStmt.Branches.ElementAt(2);
            Assert.IsType<BoolExpression>(third.Condition);
            Assert.True(third.Block.Statements.Count == 1);
            Assert.IsType<IfStatement>(third.Block.Statements.First());

            var thirdNestedIf = third.Block.Statements.First() as IfStatement;
            Assert.True(thirdNestedIf.Branches.Count == 2);
            Assert.True(thirdNestedIf.ElseBranch == null);
            Assert.IsType<BoolExpression>(thirdNestedIf.Branches.First().Condition);
            Assert.True(thirdNestedIf.Branches.First().Block.Statements.Count == 1);
            Assert.IsType<AssignmentStatement>(thirdNestedIf.Branches.First().Block.Statements.First());
            Assert.IsType<BoolExpression>(thirdNestedIf.Branches.ElementAt(1).Condition);
            Assert.True(thirdNestedIf.Branches.ElementAt(1).Block.Statements.Count == 1);
            Assert.IsType<AssignmentStatement>(thirdNestedIf.Branches.ElementAt(1).Block.Statements.First());

            Assert.True(ifStmt.ElseBranch != null);
            Assert.True(ifStmt.ElseBranch.Statements.Count == 1);
            Assert.IsType<AssignmentStatement>(ifStmt.ElseBranch.Statements.First());
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

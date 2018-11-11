﻿using System;
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
        public void ParseForLoop()
        {
            var program = File.ReadAllText("TestPrograms/Parser/for.rila");
            var lexer = new Lexer(program);
            var parser = new RilaParser(lexer);

            var ast = parser.Parse();
            Assert.True(ast.Statements.Count == 2);

            Assert.IsType<ForLoopStatement>(ast.Statements.First());
            var forLoop = ast.Statements.First() as ForLoopStatement;
            Assert.True(forLoop.VariableName == "i");

            Assert.IsType<BinaryOperatorExpression>(forLoop.InExpression);
            var forExpression = forLoop.InExpression as BinaryOperatorExpression;
            Assert.IsType<NumberExpression>(forExpression.Lhs);
            Assert.True((forExpression.Lhs as NumberExpression).Value == 0);
            Assert.IsType<NumberExpression>(forExpression.Rhs);
            Assert.True((forExpression.Rhs as NumberExpression).Value == 100);

            Assert.True(forLoop.Block.Statements.Count == 1);
            Assert.IsType<IfStatement>(forLoop.Block.Statements.First());
            var ifStmt = forLoop.Block.Statements.First() as IfStatement;
            Assert.True(ifStmt.Branches.Count == 1);

            Assert.IsType<ForLoopStatement>(ifStmt.Branches.First().Block.Statements.First());
            var nestedFor = ifStmt.Branches.First().Block.Statements.First() as ForLoopStatement;
            Assert.True(nestedFor.VariableName == "j");
            Assert.IsType<BinaryOperatorExpression>(nestedFor.InExpression);
            var nestedForExpression = nestedFor.InExpression as BinaryOperatorExpression;
            Assert.IsType<NumberExpression>(nestedForExpression.Lhs);
            Assert.True((nestedForExpression.Lhs as NumberExpression).Value == 0);
            Assert.IsType<IdentifierExpression>(nestedForExpression.Rhs);
            Assert.True((nestedForExpression.Rhs as IdentifierExpression).Name == "i");
            Assert.True(nestedFor.Block.Statements.Count == 2);
            Assert.IsType<FieldAccessExpression>(nestedFor.Block.Statements.First());
            Assert.IsType<FieldAccessExpression>(nestedFor.Block.Statements.ElementAt(1));

            Assert.True(ifStmt.ElseBranch.Statements.Count == 1);
            Assert.IsType<ContinueStatement>(ifStmt.ElseBranch.Statements.First());

            Assert.IsType<AssignmentStatement>(ast.Statements.ElementAt(1));
        }

        [Fact]
        public void ParseFieldAccess()
        {
            var program = File.ReadAllText("TestPrograms/Parser/field-access.rila");
            var lexer = new Lexer(program);
            var parser = new RilaParser(lexer);

            var ast = parser.Parse();

            Assert.True(ast.Statements.Count == 2);

            Assert.IsType<CallExpression>(ast.Statements.First());
            var first = ast.Statements.First() as CallExpression;
            Assert.IsType<IdentifierExpression>(first.Function);
            Assert.True(first.Arguments.Count == 1);
            Assert.IsType<FieldAccessExpression>(first.Arguments.First());
            var arg = first.Arguments.First() as FieldAccessExpression;
            Assert.IsType<IdentifierExpression>(arg.Lhs);

            Assert.IsType<FieldAccessExpression>(arg.Rhs);
            var inner = arg.Rhs as FieldAccessExpression;
            Assert.IsType<CallExpression>(inner.Lhs);
            Assert.IsType<IdentifierExpression>(inner.Rhs);

            Assert.IsType<AssignmentStatement>(ast.Statements.ElementAt(1));
            var assign = ast.Statements.ElementAt(1) as AssignmentStatement;
            Assert.True(assign.Identifier == "myVar");

            Assert.IsType<FieldAccessExpression>(assign.Rhs);
            var assignFieldAccess = assign.Rhs as FieldAccessExpression;
            Assert.IsType<IdentifierExpression>(assignFieldAccess.Lhs);

            Assert.IsType<FieldAccessExpression>(assignFieldAccess.Rhs);
            var assignInner1 = assignFieldAccess.Rhs as FieldAccessExpression;
            Assert.IsType<CallExpression>(assignInner1.Lhs);

            Assert.IsType<FieldAccessExpression>(assignInner1.Rhs);
            var assignInner2 = assignInner1.Rhs as FieldAccessExpression;
            Assert.IsType<IdentifierExpression>(assignInner2.Lhs);

            Assert.IsType<FieldAccessExpression>(assignInner2.Rhs);
            var assignInner3 = assignInner2.Rhs as FieldAccessExpression;
            Assert.IsType<IdentifierExpression>(assignInner3.Lhs);
            Assert.IsType<CallExpression>(assignInner3.Rhs);
        }

        [Fact]
        public void ParseFunctionCall()
        {
            var program = File.ReadAllText("TestPrograms/Parser/function-call.rila");
            var lexer = new Lexer(program);
            var parser = new RilaParser(lexer);

            var ast = parser.Parse();
            Assert.True(ast.Statements.Count == 3);

            Assert.IsType<AssignmentStatement>(ast.Statements.First());
            var assign = ast.Statements.First() as AssignmentStatement;

            Assert.IsType<CallExpression>(assign.Rhs);
            var call = assign.Rhs as CallExpression;
            Assert.IsType<IdentifierExpression>(call.Function);
            Assert.True((call.Function as IdentifierExpression).Name == "myFun");
            Assert.True(call.Arguments.Count == 4);
            Assert.IsType<NumberExpression>(call.Arguments.First());
            Assert.IsType<BinaryOperatorExpression>(call.Arguments.ElementAt(1));

            Assert.IsType<CallExpression>(call.Arguments.ElementAt(2));
            var nestedCall = call.Arguments.ElementAt(2) as CallExpression;
            Assert.IsType<IdentifierExpression>(nestedCall.Function);
            Assert.True((nestedCall.Function as IdentifierExpression).Name == "funCall");
            Assert.True(nestedCall.Arguments.Count == 1);
            Assert.IsType<NumberExpression>(nestedCall.Arguments.First());

            Assert.IsType<IdentifierExpression>(call.Arguments.ElementAt(3));

            Assert.IsType<CallExpression>(ast.Statements.ElementAt(1));
            var secondCall = ast.Statements.ElementAt(1) as CallExpression;
            Assert.True(secondCall.Arguments.Count == 0);
            Assert.IsType<IdentifierExpression>(secondCall.Function);
            Assert.True((secondCall.Function as IdentifierExpression).Name == "result");

            Assert.IsType<CallExpression>(ast.Statements.ElementAt(2));
            var thirdCall = ast.Statements.ElementAt(2) as CallExpression;
            Assert.True(thirdCall.Arguments.Count == 1);
            Assert.IsType<BinaryOperatorExpression>(thirdCall.Arguments.First());
            Assert.IsType<IdentifierExpression>(thirdCall.Function);
            Assert.True((thirdCall.Function as IdentifierExpression).Name == "oneMore");
        }

        [Fact]
        public void ParseAssignmentStatement()
        {
            var statement = "a = a + 1";
            var lexer = new Lexer(statement);
            var parser = new RilaParser(lexer);

            var ast = parser.Parse();
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

﻿using System.IO;
using Xunit;
using Xunit.Sdk;
using RilaLang.Compiler;

namespace RilaLang.Tests
{
    public class LexerTests
    {
        [Fact]
        public void ParseProgram()
        {
            var program = File.ReadAllText("TestPrograms/fib.rila");
            var lexer = new Lexer(program);
            var tokens = new Token[] //Column numbers are not tested
            {
                new Token(TokenType.Function, "fun", 1, 0),
                new Token(TokenType.WhiteSpace, string.Empty, 1, 0),
                new Token(TokenType.Identifier, "fib", 1, 6),
                new Token(TokenType.LParen, "(", 1, 7),
                new Token(TokenType.Int, "int", 1, 10),
                new Token(TokenType.WhiteSpace, string.Empty, 1, 0),
                new Token(TokenType.Identifier, "n", 1, 12),
                new Token(TokenType.RParen, ")", 1, 13),
                new Token(TokenType.WhiteSpace, string.Empty, 1, 0),
                new Token(TokenType.Arrow, "->", 1, 15),
                new Token(TokenType.WhiteSpace, string.Empty, 1, 0),
                new Token(TokenType.Int, "int", 1, 17),
                new Token(TokenType.NewLine, "\r\n", 1, 18),
                new Token(TokenType.WhiteSpace, string.Empty, 2, 0),
                new Token(TokenType.Identifier, "a", 2, 5),
                new Token(TokenType.WhiteSpace, string.Empty, 2, 0),
                new Token(TokenType.Is, "is", 2, 7),
                new Token(TokenType.WhiteSpace, string.Empty, 2, 0),
                new Token(TokenType.NumericLiteral, "0", 2, 9),
                new Token(TokenType.NewLine, "\r\n", 2, 10),
                new Token(TokenType.WhiteSpace, string.Empty, 3, 0),
                new Token(TokenType.Identifier, "b", 3, 5),
                new Token(TokenType.WhiteSpace, string.Empty, 3, 0),
                new Token(TokenType.Is, "is", 3, 7),
                new Token(TokenType.WhiteSpace, string.Empty, 3, 0),
                new Token(TokenType.NumericLiteral, "1", 3, 9),
                new Token(TokenType.NewLine, "\r\n", 3, 10),
                new Token(TokenType.WhiteSpace, string.Empty, 4, 0),
                new Token(TokenType.Identifier, "test", 4, 0),
                new Token(TokenType.WhiteSpace, string.Empty, 4, 0),
                new Token(TokenType.Is, "is", 4, 0),
                new Token(TokenType.WhiteSpace, string.Empty, 4, 0),
                new Token(TokenType.StringLiteral, "test \"string\"", 4, 0),
                new Token(TokenType.NewLine, "\r\n", 4, 0),
                new Token(TokenType.WhiteSpace, string.Empty, 5, 0),
                new Token(TokenType.For, "for", 5, 6),
                new Token(TokenType.WhiteSpace, string.Empty, 5, 0),
                new Token(TokenType.Identifier, "i", 5, 8),
                new Token(TokenType.WhiteSpace, string.Empty, 5, 0),
                new Token(TokenType.In, "in", 5, 10),
                new Token(TokenType.WhiteSpace, string.Empty, 5, 0),
                new Token(TokenType.NumericLiteral, "0", 5, 12),
                new Token(TokenType.Range, "..", 5, 13),
                new Token(TokenType.Identifier, "n", 5, 14),
                new Token(TokenType.NewLine, "\r\n", 5, 15),
                new Token(TokenType.WhiteSpace, string.Empty, 6, 0),
                new Token(TokenType.Identifier, "temp", 6, 6),
                new Token(TokenType.WhiteSpace, string.Empty, 6, 0),
                new Token(TokenType.Assign, "=", 6, 8),
                new Token(TokenType.WhiteSpace, string.Empty, 6, 0),
                new Token(TokenType.Identifier, "a", 6, 10),
                new Token(TokenType.NewLine, "\r\n", 6, 11),
                new Token(TokenType.WhiteSpace, string.Empty, 7, 0),
                new Token(TokenType.Identifier, "a", 7, 6),
                new Token(TokenType.WhiteSpace, string.Empty, 7, 0),
                new Token(TokenType.Assign, "=", 7, 8),
                new Token(TokenType.WhiteSpace, string.Empty, 7, 0),
                new Token(TokenType.Identifier, "b", 7, 10),
                new Token(TokenType.NewLine, "\r\n", 7, 11),
                new Token(TokenType.WhiteSpace, string.Empty, 8, 0),
                new Token(TokenType.Identifier, "b", 8, 6),
                new Token(TokenType.WhiteSpace, string.Empty, 8, 0),
                new Token(TokenType.Assign, "=", 8, 8),
                new Token(TokenType.WhiteSpace, string.Empty, 8, 0),
                new Token(TokenType.Identifier, "temp", 8, 10),
                new Token(TokenType.WhiteSpace, string.Empty, 8, 0),
                new Token(TokenType.Plus, "+", 8, 12),
                new Token(TokenType.WhiteSpace, string.Empty, 8, 0),
                new Token(TokenType.Identifier, "b", 8, 14),
                new Token(TokenType.NewLine, "\r\n", 8, 11),
                new Token(TokenType.NewLine, "\r\n", 9, 0),
                new Token(TokenType.WhiteSpace, string.Empty, 10, 0),
                new Token(TokenType.Return, "return", 10, 3),
                new Token(TokenType.WhiteSpace, string.Empty, 10, 0),
                new Token(TokenType.Identifier, "a", 10, 5),
                new Token(TokenType.NewLine, "\r\n", 10, 6),
                new Token(TokenType.EOF, string.Empty, 11, 0)
            };

            var i = 0;
            while(!lexer.AtEof)
            {
                var match = tokens[i];
                var token = lexer.NextToken();

                if (!TokenEquals(token, match))
                    throw new XunitException($"Token expected: {match}, got: {token}");

                i++;
            }
        }

        private bool TokenEquals(Token lhs, Token rhs)
        {
            return lhs.TokenType == rhs.TokenType &&
                   lhs.Content == rhs.Content &&
                   lhs.Line == rhs.Line;
        }
    }
}

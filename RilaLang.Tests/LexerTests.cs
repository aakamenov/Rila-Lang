using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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
            var tokens = new Token[]
            {
                new Token(TokenType.Function, "fun", 1, 0),
                new Token(TokenType.Identifier, "fib", 1, 6),
                new Token(TokenType.LParen, "(", 1, 7),
                new Token(TokenType.Int, "int", 1, 10),
                new Token(TokenType.Identifier, "n", 1, 12),
                new Token(TokenType.RParen, ")", 1, 13),
                new Token(TokenType.Arrow, "->", 1, 15),
                new Token(TokenType.Int, "int", 1, 17),
                new Token(TokenType.NewLine, "\n\r", 1, 18),
                new Token(TokenType.Tab, "\t", 2, 0),
                new Token(TokenType.Identifier, "a", 2, 5),
                new Token(TokenType.Is, "is", 2, 7),
                new Token(TokenType.NumericLiteral, "0", 2, 9),
                new Token(TokenType.NewLine, "\n\r", 2, 10),
                new Token(TokenType.Tab, "\t", 3, 0),
                new Token(TokenType.Identifier, "b", 3, 5),
                new Token(TokenType.Is, "is", 3, 7),
                new Token(TokenType.NumericLiteral, "1", 3, 9),
                new Token(TokenType.NewLine, "\n\r", 3, 10),
                new Token(TokenType.NewLine, "\n\r", 4, 0),
                new Token(TokenType.Tab, "\t", 5, 0),
                new Token(TokenType.For, "for", 5, 6),
                new Token(TokenType.Identifier, "i", 5, 8),
                new Token(TokenType.In, "in", 5, 10),
                new Token(TokenType.NumericLiteral, "0", 5, 12),
                new Token(TokenType.Range, "..", 5, 13),
                new Token(TokenType.Identifier, "n", 5, 14),
                new Token(TokenType.NewLine, "\n\r", 5, 15),
                new Token(TokenType.Tab, "\t", 6, 0),
                new Token(TokenType.Tab, "\t", 6, 3),
                new Token(TokenType.Identifier, "temp", 6, 6),
                new Token(TokenType.Assign, "=", 6, 8),
                new Token(TokenType.Identifier, "a", 6, 10),
                new Token(TokenType.NewLine, "\n\r", 6, 11),
                new Token(TokenType.Tab, "\t", 7, 0),
                new Token(TokenType.Tab, "\t", 7, 3),
                new Token(TokenType.Identifier, "a", 7, 6),
                new Token(TokenType.Assign, "=", 7, 8),
                new Token(TokenType.Identifier, "b", 7, 10),
                new Token(TokenType.NewLine, "\n\r", 7, 11),
                new Token(TokenType.Tab, "\t", 8, 0),
                new Token(TokenType.Tab, "\t", 8, 3),
                new Token(TokenType.Identifier, "b", 8, 6),
                new Token(TokenType.Assign, "=", 8, 8),
                new Token(TokenType.Identifier, "temp", 8, 10),
                new Token(TokenType.Plus, "+", 8, 12),
                new Token(TokenType.Identifier, "b", 8, 14),
                new Token(TokenType.NewLine, "\n\r", 8, 11),
                new Token(TokenType.NewLine, "\n\r", 9, 0),
                new Token(TokenType.Tab, "\t", 10, 0),
                new Token(TokenType.Return, "return", 10, 3),
                new Token(TokenType.Identifier, "a", 10, 5),
                new Token(TokenType.NewLine, "\n\r", 10, 6),
                new Token(TokenType.EOF, string.Empty, 11, 0)
            };

            var i = 0;
            foreach(var token in lexer)
            {
                var match = tokens[i];

                if (!TokenEquals(token, match))
                    throw new XunitException($"Token expected: {match}, got: {token.TokenType}");

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

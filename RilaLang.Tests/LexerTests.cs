using System;
using System.IO;
using Xunit;
using Xunit.Sdk;
using RilaLang.Compiler;

namespace RilaLang.Tests
{
    public class LexerTests
    {
        [Fact]
        public void LexProgram()
        {
            var program = File.ReadAllText("TestPrograms/Lexer/fib.rila");
            var lexer = new Lexer(program);
            var tokens = new Token[] //Column numbers are not tested
            {
                new Token(TokenType.Function, "fun", 0, 1, 0),
                new Token(TokenType.Identifier, "fib", 0, 1, 6),
                new Token(TokenType.LParen, "(", 0, 1, 7),
                new Token(TokenType.Identifier, "int", 0, 1, 10),
                new Token(TokenType.Identifier, "n", 0, 1, 12),
                new Token(TokenType.RParen, ")", 0, 1, 13),
                new Token(TokenType.Arrow, "->", 0, 1, 15),
                new Token(TokenType.Identifier, "int", 0, 1, 17),
                new Token(TokenType.NewLine, "\r\n", 0, 1, 18),

                new Token(TokenType.Identifier, "a", 4, 2, 5),
                new Token(TokenType.Assign, "=", 4, 2, 7),
                new Token(TokenType.NumericLiteral, "0", 4, 2, 9),
                new Token(TokenType.NewLine, "\r\n", 4, 2, 10),
                new Token(TokenType.Identifier, "b", 4, 3, 5),
                new Token(TokenType.Assign, "=", 4, 3, 7),
                new Token(TokenType.NumericLiteral, "1", 4, 3, 9),
                new Token(TokenType.NewLine, "\r\n", 4, 3, 10),
                new Token(TokenType.Identifier, "test", 4, 4, 0),
                new Token(TokenType.Assign, "=", 4, 4, 0),
                new Token(TokenType.StringLiteral, "test \"string\"", 4, 4, 0),
                new Token(TokenType.NewLine, "\r\n", 4, 4, 0),
                new Token(TokenType.For, "for", 4, 5, 6),
                new Token(TokenType.Identifier, "i", 4, 5, 8),
                new Token(TokenType.In, "in", 4, 5, 10),
                new Token(TokenType.NumericLiteral, "0", 4, 5, 12),
                new Token(TokenType.Range, "..", 4, 5, 13),
                new Token(TokenType.Identifier, "n", 4, 5, 14),
                new Token(TokenType.NewLine, "\r\n", 4, 5, 15),

                new Token(TokenType.Identifier, "temp", 8, 6, 6),
                new Token(TokenType.Assign, "=", 8, 6, 8),
                new Token(TokenType.Identifier, "a", 8, 6, 10),
                new Token(TokenType.NewLine, "\r\n", 8, 6, 11),
                new Token(TokenType.Identifier, "a", 8, 7, 6),
                new Token(TokenType.Assign, "=", 8, 7, 8),
                new Token(TokenType.Identifier, "b", 8, 7, 10),
                new Token(TokenType.NewLine, "\r\n", 8, 7, 11),
                new Token(TokenType.Identifier, "b", 8, 8, 6),
                new Token(TokenType.Assign, "=", 8, 8, 8),
                new Token(TokenType.Identifier, "temp", 8, 8, 10),
                new Token(TokenType.Plus, "+", 8, 8, 12),
                new Token(TokenType.Identifier, "b", 8, 8, 14),
                new Token(TokenType.NewLine, "\r\n", 8, 8, 11),
                new Token(TokenType.NewLine, "\r\n", 0, 9, 0),

                new Token(TokenType.Return, "return", 4, 10, 3),
                new Token(TokenType.Identifier, "a", 4, 10, 5),
                new Token(TokenType.NewLine, "\r\n", 4, 10, 6),
                new Token(TokenType.EOF, string.Empty, 0, 11, 0)
            };

            AssertTokens(lexer, tokens);
        }

        [Fact]
        public void LexIf()
        {
            var program = File.ReadAllText("TestPrograms/Lexer/if.rila");
            var lexer = new Lexer(program);
            var tokens = new Token[] //Column numbers are not tested
            {
                new Token(TokenType.If, "if", 0, 1, 0),
                new Token(TokenType.StringLiteral, "string", 0, 1, 0),
                new Token(TokenType.Equal, "==", 0, 1, 0),
                new Token(TokenType.NumericLiteral, "5", 0, 1, 0),
                new Token(TokenType.NewLine, "\r\n", 0, 1, 0),

                new Token(TokenType.Identifier, "x", 4, 2, 0),
                new Token(TokenType.Assign, "=", 4, 2, 0),
                new Token(TokenType.True, "true", 4, 2, 0),
                new Token(TokenType.NewLine, "\r\n", 4, 2, 0),
                new Token(TokenType.NewLine, "\r\n", 0, 3, 0),
                new Token(TokenType.EOF, string.Empty, 0, 4, 0)
            };

            AssertTokens(lexer, tokens);
        }

        [Fact]
        public void UnrecognisedCharacterThrows()
        {
            void Iterate(Lexer lexer)
            {
                while(lexer.NextToken().TokenType != TokenType.EOF) { }
            }

            var code = "a = 10;";
            Action action = () => Iterate(new Lexer(code));

            Assert.Throws<RilaParserException>(action);
        }

        private void AssertTokens(Lexer lexer, Token[] testTokens)
        {
            var i = 0;

            while (!lexer.AtEof)
            {
                var match = testTokens[i];
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
                   lhs.Line == rhs.Line &&
                   lhs.IndentationLevel == rhs.IndentationLevel;
        }
    }
}

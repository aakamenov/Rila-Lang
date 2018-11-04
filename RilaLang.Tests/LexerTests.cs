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
            var program = File.ReadAllText("TestPrograms/fib.rila");
            var lexer = new Lexer(program);
            var tokens = new Token[] //Column numbers are not tested
            {
                new Token(TokenType.Function, "fun", 1, 0),
                new Token(TokenType.Identifier, "fib", 1, 6),
                new Token(TokenType.LParen, "(", 1, 7),
                new Token(TokenType.Int, "int", 1, 10),
                new Token(TokenType.Identifier, "n", 1, 12),
                new Token(TokenType.RParen, ")", 1, 13),
                new Token(TokenType.Arrow, "->", 1, 15),
                new Token(TokenType.Int, "int", 1, 17),
                new Token(TokenType.NewLine, "\r\n", 1, 18),
                new WSToken(4, 2, 0),
                new Token(TokenType.Identifier, "a", 2, 5),
                new Token(TokenType.Is, "is", 2, 7),
                new Token(TokenType.NumericLiteral, "0", 2, 9),
                new Token(TokenType.NewLine, "\r\n", 2, 10),
                new WSToken(4, 3, 0),
                new Token(TokenType.Identifier, "b", 3, 5),
                new Token(TokenType.Is, "is", 3, 7),
                new Token(TokenType.NumericLiteral, "1", 3, 9),
                new Token(TokenType.NewLine, "\r\n", 3, 10),
                new WSToken(4, 4, 0),
                new Token(TokenType.Identifier, "test", 4, 0),
                new Token(TokenType.Is, "is", 4, 0),
                new Token(TokenType.StringLiteral, "test \"string\"", 4, 0),
                new Token(TokenType.NewLine, "\r\n", 4, 0),
                new WSToken(4, 5, 0),
                new Token(TokenType.For, "for", 5, 6),
                new Token(TokenType.Identifier, "i", 5, 8),
                new Token(TokenType.In, "in", 5, 10),
                new Token(TokenType.NumericLiteral, "0", 5, 12),
                new Token(TokenType.Range, "..", 5, 13),
                new Token(TokenType.Identifier, "n", 5, 14),
                new Token(TokenType.NewLine, "\r\n", 5, 15),
                new WSToken(8, 6, 0),
                new Token(TokenType.Identifier, "temp", 6, 6),
                new Token(TokenType.Assign, "=", 6, 8),
                new Token(TokenType.Identifier, "a", 6, 10),
                new Token(TokenType.NewLine, "\r\n", 6, 11),
                new WSToken(8, 7, 0),
                new Token(TokenType.Identifier, "a", 7, 6),
                new Token(TokenType.Assign, "=", 7, 8),
                new Token(TokenType.Identifier, "b", 7, 10),
                new Token(TokenType.NewLine, "\r\n", 7, 11),
                new WSToken(8, 8, 0),
                new Token(TokenType.Identifier, "b", 8, 6),
                new Token(TokenType.Assign, "=", 8, 8),
                new Token(TokenType.Identifier, "temp", 8, 10),
                new Token(TokenType.Plus, "+", 8, 12),
                new Token(TokenType.Identifier, "b", 8, 14),
                new Token(TokenType.NewLine, "\r\n", 8, 11),
                new Token(TokenType.NewLine, "\r\n", 9, 0),
                new WSToken(4, 10, 0),
                new Token(TokenType.Return, "return", 10, 3),
                new Token(TokenType.Identifier, "a", 10, 5),
                new Token(TokenType.NewLine, "\r\n", 10, 6),
                new Token(TokenType.EOF, string.Empty, 11, 0)
            };

            AssertTokens(lexer, tokens);
        }

        [Fact]
        public void LexIf()
        {
            var program = File.ReadAllText("TestPrograms/if.rila");
            var lexer = new Lexer(program);
            var tokens = new Token[] //Column numbers are not tested
            {
                new Token(TokenType.If, "if", 1, 0),
                new Token(TokenType.StringLiteral, "string", 1, 0),
                new Token(TokenType.Equal, "==", 1, 0),
                new Token(TokenType.NumericLiteral, "5", 1, 0),
                new Token(TokenType.NewLine, "\r\n", 1, 0),
                new WSToken(4, 2, 0),
                new Token(TokenType.Identifier, "x", 2, 0),
                new Token(TokenType.Is, "is", 2, 0),
                new Token(TokenType.True, "true", 2, 0),
                new Token(TokenType.NewLine, "\r\n", 2, 0),
                new Token(TokenType.NewLine, "\r\n", 3, 0),
                new Token(TokenType.EOF, string.Empty, 4, 0)
            };

            AssertTokens(lexer, tokens);
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

                if(match is WSToken)
                {
                    var matchWS = match as WSToken;
                    var tokenWS = token as WSToken;

                    Assert.True(matchWS.IndentationLevel == tokenWS.IndentationLevel);
                }

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

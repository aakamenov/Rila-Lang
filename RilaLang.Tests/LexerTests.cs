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
                new Token(TokenType.Function, string.Empty, 1, 0)
            };

            var i = 0;
            foreach(var token in lexer)
            {
                var match = tokens[i].TokenType;

                if (token.TokenType != match)
                    throw new XunitException($"Token expected: {match}, got: {token.TokenType}");

                i++;
            }
        }
    }
}

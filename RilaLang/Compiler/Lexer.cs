using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RilaLang.Compiler
{
    public class Lexer : IEnumerable<Token>
    {
        private readonly string source;
        private readonly string fileName;

        private int position;
        private int currentLine;
        private int currentColumn;
        private readonly int sourceLength;

        private bool atEof => position >= sourceLength;

        public Lexer(string source, string fileName = null)
        {
            this.source = source;
            this.fileName = fileName;
            sourceLength = source.Length;
            currentLine = 1;
        }

        public IEnumerator<Token> GetEnumerator()
        {
            if (atEof)
            {
                yield return new Token(TokenType.EOF, string.Empty, currentLine, currentColumn);
                yield break;
            }

            var next = source[position];
            currentColumn++;

            switch(next)
            {
                case '\"':
                    yield return new Token(TokenType.StringLiteral, ReadStringLiteral(), currentLine, currentColumn);
                    break;
                case '\n':
                case '\r':
                    var peeked = PeekChar(out char peekChar);
                    var builder = new StringBuilder(next.ToString());

                    if (peeked && (peekChar == '\n' || peekChar == '\r'))
                    {
                        position++;
                        builder.Append(peekChar);
                    }
                    
                    yield return new Token(TokenType.NewLine, builder.ToString(), currentLine, currentColumn);
                    break;
                    
                default:
                    yield return new Token(TokenType.EOF, string.Empty, currentLine, currentColumn);
                    break;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private bool PeekChar(out char next)
        {
            if (position + 1 >= sourceLength)
            {
                next = char.MinValue;
                return false;
            }

            next = source[position + 1];
            return true;
        }

        private string ReadStringLiteral()
        {
            var builder = new StringBuilder();

            while(true)
            {
                position++;

                if (atEof)
                    throw new RilaParserException($"Missing closing string literal quote ({currentLine}, {currentColumn})");

                var ch = source[position];

                if (ch == '\\')
                {
                    if (PeekChar(out char next) && next == '\"')
                    {
                        builder.Append(ch);
                        builder.Append(next);
                        position++;
                    }
                }
                else if (ch == '\"')
                {
                    position++;
                    break;
                }
                else
                    builder.Append(ch);
                           
            }

            return builder.ToString();
        }
    }
}

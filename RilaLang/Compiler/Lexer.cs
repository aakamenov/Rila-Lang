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
        private uint currentLine;
        private uint currentColumn;
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

            switch(next)
            {
                case '\"':
                    yield return new Token(TokenType.StringLiteral, ReadStringLiteral(), currentLine, currentColumn);
                    break;
                case '\n':
                case '\r':
                    { 
                        var peeked = PeekChar(out char peekChar);
                        var builder = new StringBuilder(next.ToString());

                        if (peeked && (peekChar == '\n' || peekChar == '\r'))
                        {
                            position++;
                            builder.Append(peekChar);
                        }
                    
                        yield return new Token(TokenType.NewLine, builder.ToString(), currentLine, currentColumn);
                        currentLine++;
                    }
                    break;
                case '\t':
                    yield return new Token(TokenType.Tab, "\t", currentLine, currentColumn);
                    break;
                case '(':
                    yield return new Token(TokenType.LParen, "(", currentLine, currentColumn);
                    break;
                case ')':
                    yield return new Token(TokenType.RParen, ")", currentLine, currentColumn);
                    break;
                case '.':
                    { 
                        if(PeekChar(out char peeked) && peeked == '.')
                        {
                            yield return new Token(TokenType.Range, "..", currentLine, currentColumn);
                            position++;
                        }
                        else
                        {
                            yield return new Token(TokenType.Dot, ".", currentLine, currentColumn);
                        }
                    }
                    break;
                case '+':
                    yield return new Token(TokenType.Plus, "+", currentLine, currentColumn);
                    break;
                case '>':
                    yield return new Token(TokenType.GreaterThan, ">", currentLine, currentColumn);
                    break;
                case '<':
                    yield return new Token(TokenType.LessThan, "<", currentLine, currentColumn);
                    break;
                case '-':
                    {
                        if (PeekChar(out char peeked) && peeked == '>')
                        {
                            yield return new Token(TokenType.Arrow, "->", currentLine, currentColumn);
                            position++;
                        }
                        else
                        {
                            yield return new Token(TokenType.Minus, "-", currentLine, currentColumn);
                        }
                    }
                    break;
                    
                default:
                    {

                        if (TryReadWord(next, out string word))
                        {
                            if (Token.TryGetKeyword(word, out TokenType tokenType))
                            {
                                yield return new Token(tokenType, word, currentLine, currentColumn);
                            }
                            else
                            {
                                yield return new Token(TokenType.Identifier, word, currentLine, currentColumn);
                            }
                        }
                        else
                            yield return new Token(TokenType.None, string.Empty, currentLine, currentColumn);
                    }
                    break;
            }

            position++;
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

        private bool TryReadWord(char first, out string word)
        {
            var builder = new StringBuilder();
            builder.Append(first);

            while(PeekChar(out char next))
            {
                if(char.IsWhiteSpace(next))
                    break;

                if (IsWordChar(next))
                { 
                    builder.Append(next);
                    position++;
                }
                else
                {
                    word = null;
                    return false;
                }
            }

            word = builder.ToString();
            return true;
        }

        private bool IsWordChar(char @char)
        {
            return char.IsLetter(@char) || @char == '_';
        }

        private string ReadStringLiteral()
        {
            var builder = new StringBuilder("\"");

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
                    break;
                }
                else
                    builder.Append(ch);                       
            }

            return builder.ToString();
        }
    }
}

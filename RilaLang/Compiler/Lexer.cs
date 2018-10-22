using System.Text;

namespace RilaLang.Compiler
{
    public class Lexer
    {
        public bool AtEof => position >= sourceLength;

        private readonly string source;
        private readonly string fileName;

        private int position;
        private uint currentLine;
        private uint currentColumn;
        private readonly int sourceLength;

        public Lexer(string source, string fileName = null)
        {
            this.source = source;
            this.fileName = fileName;
            sourceLength = source.Length;
            currentLine = 1;
        }

        public Token NextToken()
        {
            if (AtEof)
            {
                return new Token(TokenType.EOF, string.Empty, currentLine, currentColumn);
            }

            var next = source[position];
            Token token = null;

            if(IsWhiteSpace(ref next))
            {
                ConsumeWhiteSpace();
                token = new Token(TokenType.WhiteSpace, string.Empty, currentLine, currentColumn);

                goto end;
            }

            switch(next)
            {
                case '\"':
                    token = new Token(TokenType.StringLiteral, ReadStringLiteral(), currentLine, currentColumn);
                    break;
                case '\r':
                case '\n':
                    { 
                        var peeked = TryPeekChar(out char peekChar);
                        var builder = new StringBuilder();
                        builder.Append(next);

                        if (peeked && (peekChar == '\n' || peekChar == '\r'))
                        {
                            position++;
                            currentColumn++;
                            builder.Append(peekChar);
                        }
                    
                        token = new Token(TokenType.NewLine, builder.ToString(), currentLine, currentColumn);
                        NewLine();
                    }
                    break;
                case '(':
                    token = new Token(TokenType.LParen, "(", currentLine, currentColumn);
                    break;
                case ')':
                    token = new Token(TokenType.RParen, ")", currentLine, currentColumn);
                    break;
                case '.':
                    { 
                        if(TryPeekChar(out char peeked) && peeked == '.')
                        {
                            token = new Token(TokenType.Range, "..", currentLine, currentColumn);
                            currentColumn++;
                            position++;
                        }
                        else
                            token = new Token(TokenType.Dot, ".", currentLine, currentColumn);
                    }
                    break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    token = new Token(TokenType.NumericLiteral, ReadNumericLiteral(next), currentLine, currentColumn);
                    break;
                case '=':
                    token = new Token(TokenType.Assign, "=", currentLine, currentColumn);
                    break;
                case '+':
                    token = new Token(TokenType.Plus, "+", currentLine, currentColumn);
                    break;
                case '>':
                    token = new Token(TokenType.GreaterThan, ">", currentLine, currentColumn);
                    break;
                case '<':
                    token = new Token(TokenType.LessThan, "<", currentLine, currentColumn);
                    break;
                case '-':
                    {
                        if (TryPeekChar(out char peeked) && peeked == '>')
                        {
                            token = new Token(TokenType.Arrow, "->", currentLine, currentColumn);
                            currentColumn++;
                            position++;
                        }
                        else
                            token = new Token(TokenType.Minus, "-", currentLine, currentColumn);
                    }
                    break;
                default:
                    {
                        var word = ReadWord(next);

                        if (Token.TryGetKeyword(word, out TokenType tokenType))
                        {
                            token = new Token(tokenType, word, currentLine, currentColumn);
                        }
                        else
                        {
                            token = new Token(TokenType.Identifier, word, currentLine, currentColumn);
                        }
                    }
                    break;
            }

            end:
                currentColumn++;
                position++;

                return token;
        }

        private bool TryPeekChar(out char next)
        {
            if (position + 1 >= sourceLength)
            {
                next = char.MinValue;
                return false;
            }

            next = source[position + 1];
            return true;
        }

        private void ConsumeWhiteSpace()
        {
            while(TryPeekChar(out char next))
            {
                if (!IsWhiteSpace(ref next))
                    break;

                position++;
            }
        }

        private bool IsWhiteSpace(ref char c)
        {
            return c == ' ' || c == '\t' || c == '\0';
        }

        private void NewLine()
        {
            currentLine++;
            currentColumn = 0;
        }

        private string ReadWord(char first)
        {
            var builder = new StringBuilder();
            builder.Append(first);

            while(TryPeekChar(out char next))
            {


                if (IsWordChar(next))
                { 
                    builder.Append(next);
                    currentColumn++;
                    position++;
                }
                else
                {
                    break;
                }
            }

            return builder.ToString();
        }

        private string ReadNumericLiteral(char first)
        {
            var builder = new StringBuilder();
            builder.Append(first);

            while(TryPeekChar(out char next))
            {
                if (char.IsDigit(next))
                {
                    builder.Append(next);
                    position++;
                }
                else
                    break;
            }

            return builder.ToString();
        }

        private bool IsWordChar(char c)
        {
            return char.IsLetter(c) || c == '_';
        }

        private string ReadStringLiteral()
        {
            var builder = new StringBuilder();

            while(true)
            {
                position++;
                currentColumn++;

                if (AtEof)
                    throw new RilaParserException($"Missing closing string literal quote ({currentLine}, {currentColumn})");

                var ch = source[position];

                if (ch == '\\')
                {
                    if (TryPeekChar(out char next) && next == '\"')
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

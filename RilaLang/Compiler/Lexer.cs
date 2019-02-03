using System.Text;

namespace RilaLang.Compiler
{
    public class Lexer
    {
        public bool AtEof => position >= sourceLength;
        public string Filename { get; }

        private readonly string source;

        private int position;
        private uint currentLine;
        private uint currentColumn;
        private uint currentIndentation;
        private readonly int sourceLength;
        private TokenType prevToken;

        internal Lexer(string source, string fileName = null)
        {
            this.source = source;
            Filename = fileName;
            sourceLength = source.Length;
            currentLine = 1;
        }

        public Token NextToken()
        {
            if (AtEof)
            {
                return new Token(TokenType.EOF, string.Empty, 0, currentLine, currentColumn);
            }

            var next = source[position];
            Token token = null;

            if(IsWhiteSpace(next))
            {
                var wsCount = ConsumeWhiteSpace();

                if (prevToken == TokenType.NewLine &&
                    TryPeekChar(out char peek) && !("\n\r".Contains(peek.ToString()))) //Don't include WS on empty lines
                {
                    currentIndentation = wsCount;
                }

                next = source[position];
            }

            switch(next)
            {
                case '\"':
                    token = new Token(TokenType.StringLiteral, ReadStringLiteral(), currentIndentation, currentLine, currentColumn);
                    break;
                case '\r':
                case '\n':
                    { 
                        var peeked = TryPeekChar(out char peekChar);
                        var builder = new StringBuilder();
                        builder.Append(next);

                        if (peeked && (peekChar == '\n' || peekChar == '\r'))
                        {
                            AdvancePosition();
                            builder.Append(peekChar);
                        }
                    
                        token = new Token(TokenType.NewLine, builder.ToString(), currentIndentation, currentLine, currentColumn);
                        NewLine();
                    }
                    break;
                case '(':
                    token = new Token(TokenType.LParen, "(", currentIndentation, currentLine, currentColumn);
                    break;
                case ')':
                    token = new Token(TokenType.RParen, ")", currentIndentation, currentLine, currentColumn);
                    break;
                case '.':
                    { 
                        if(TryPeekChar(out char peeked) && peeked == '.')
                        {
                            token = new Token(TokenType.Range, "..", currentIndentation, currentLine, currentColumn);
                            AdvancePosition();
                        }
                        else
                            token = new Token(TokenType.Dot, ".", currentIndentation, currentLine, currentColumn);
                    }
                    break;
                case ',':
                    token = new Token(TokenType.Comma, ",", currentIndentation, currentLine, currentColumn);
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
                    token = new Token(TokenType.NumericLiteral, ReadNumericLiteral(next), currentIndentation, currentLine, currentColumn);
                    break;
                case '!':
                    {
                        if (TryPeekChar(out char peeked) && peeked == '=')
                        {
                            token = new Token(TokenType.NotEqual, "!=", currentIndentation, currentLine, currentColumn);
                            AdvancePosition();
                        }
                        else
                            throw new RilaParserException($"Expecting \"=\" after a \"!\" on line {currentLine}:{currentColumn}");
                    }
                    break;
                case '=':
                    {
                        if (TryPeekChar(out char peeked) && peeked == '=')
                        {
                            token = new Token(TokenType.Equal, "==", currentIndentation, currentLine, currentColumn);
                            AdvancePosition();
                        }
                        else
                            token = new Token(TokenType.Assign, "=", currentIndentation, currentLine, currentColumn);
                    }
                    break;
                case '+':
                    token = new Token(TokenType.Plus, "+", currentIndentation, currentLine, currentColumn);
                    break;
                case '>':
                    {
                        if (TryPeekChar(out char peeked) && peeked == '=')
                        {
                            token = new Token(TokenType.EqGreaterThan, ">=", currentIndentation, currentLine, currentColumn);
                            AdvancePosition();
                        }
                        else
                            token = new Token(TokenType.GreaterThan, ">", currentIndentation, currentLine, currentColumn);
                    }
                    break;
                case '<':
                    {
                        if (TryPeekChar(out char peeked) && peeked == '=')
                        {
                            token = new Token(TokenType.EqLessThan, "=<", currentIndentation, currentLine, currentColumn);
                        }
                        else
                            token = new Token(TokenType.LessThan, "<", currentIndentation, currentLine, currentColumn);
                    }
                    break;
                case '-':
                    {
                        if (TryPeekChar(out char peeked) && peeked == '>')
                        {
                            token = new Token(TokenType.Arrow, "->", currentIndentation, currentLine, currentColumn);
                            AdvancePosition();
                        }
                        else
                            token = new Token(TokenType.Minus, "-", currentIndentation, currentLine, currentColumn);
                    }
                    break;
                case '/':
                    token = new Token(TokenType.Slash, "/", currentIndentation, currentLine, currentColumn);
                    break;
                case '*':
                    token = new Token(TokenType.Asterisk, "*", currentIndentation, currentLine, currentColumn);
                    break;
                case '%':
                    token = new Token(TokenType.Modulo, "%", currentIndentation, currentLine, currentColumn);
                    break;
                case '[':
                    token = new Token(TokenType.LSquare, "[", currentIndentation, currentLine, currentColumn);
                    break;
                case ']':
                    token = new Token(TokenType.RSquare, "]", currentIndentation, currentLine, currentColumn);
                    break;
                default:
                    {
                        if (!IsWordChar(next))
                            throw new RilaParserException($"Unexpected token \"{next}\"!");

                        var word = ReadWord(next);

                        if (Token.TryGetKeyword(word, out TokenType tokenType))
                        {
                            token = new Token(tokenType, word, currentIndentation, currentLine, currentColumn);
                        }
                        else
                        {
                            token = new Token(TokenType.Identifier, word, currentIndentation, currentLine, currentColumn);
                        }
                    }
                    break;
            }

            AdvancePosition();
            prevToken = token.TokenType;

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

        private uint ConsumeWhiteSpace()
        {
            var consumed = 0u;
            var current = source[position];

            while(IsWhiteSpace(current))
            {
                consumed++;

                if (TryPeekChar(out char next))
                    current = next;
                else
                    break;

                AdvancePosition();
            }

            return consumed;
        }

        private bool IsWhiteSpace(char c)
        {
            return c == ' ' || c == '\t' || c == '\0';
        }

        private void NewLine()
        {
            currentLine++;
            currentColumn = 0;
            currentIndentation = 0;
        }

        private void AdvancePosition()
        {
            position++;
            currentColumn++;
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
                    AdvancePosition();
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
                    AdvancePosition();
                }
                else
                    break;
            }

            return builder.ToString();
        }

        private bool IsWordChar(char c)
        {
            return char.IsLetter(c) || char.IsNumber(c) || c == '_';
        }

        private string ReadStringLiteral()
        {
            var builder = new StringBuilder();

            while(true)
            {
                AdvancePosition();

                if (AtEof)
                    throw new RilaParserException($"Missing closing string literal quote ({currentLine}, {currentColumn})");

                var ch = source[position];

                if (ch == '\\')
                {
                    if (TryPeekChar(out char next) && next == '\"')
                    {
                        builder.Append(next);
                        AdvancePosition();
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

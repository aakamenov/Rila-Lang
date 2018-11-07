using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RilaLang.Compiler.Ast;
using RilaLang.Compiler.Parselets;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RilaLang.Tests")]
namespace RilaLang.Compiler
{
    public class RilaParser : ParserBase
    {
        private Lexer lexer;
        private List<Token> read;
        private StringBuilder errorSink;
        private uint currentIndentationLevel;

        public RilaParser(Lexer lexer) : base()
        {
            this.lexer = lexer;

            read = new List<Token>();
            errorSink = new StringBuilder();
        }
        
        public AstNode Parse()
        {
            AstNode root = null;

            try
            {
                root = ParseStatement();
            }
            catch(RilaParserException e)
            {
                errorSink.AppendLine(e.Message);
            }
            finally
            {
                if (errorSink.Length > 0)
                    throw new RilaParserException(errorSink.ToString());
            }

            return root;
        }

        internal AstNode ParseStatement()
        {
            var token = Peek();
            var statements = new List<AstNode>();

            ConsumeWhiteSpace();

            while (token.TokenType != TokenType.EOF)
            {
                switch (token.TokenType)
                {
                    case TokenType.Identifier:
                        statements.Add(ParseAssignment());
                        break;
                    default:
                        statements.Add(ParseExpression());
                        break;
                }

                ConsumeWhiteSpace();
                token = Peek();
            }

            throw new NotImplementedException();
        }

        internal Expression ParseExpression(Precedence precedence = Precedence.None)
        {
            var token = Consume();
            Expression left = null;

            if (prefixParselets.TryGetValue(token.TokenType, out IPrefixParselet prefixParselet))
            {
                left = prefixParselet.Parse(this, token);
            }
            else
                throw new RilaParserException($"Unrecognised token: {token.Content}");

            while (precedence < GetPrecedence())
            {
                token = Consume();

                if (infixParselets.TryGetValue(token.TokenType, out IInfixParselet infixParselet))
                {
                    left = infixParselet.Parse(this, token, left);
                }
                else
                    throw new RilaParserException($"Unrecognised token: {token.Content}");
            }

            return left;
        }

        internal bool Expect(TokenType type, out Token match)
        {
            var token = Peek();

            if (token.TokenType != type)
            {
                AppendError($"Expecting '{type.ToString()}', found {token.Content}", token);
                match = null;

                return false;
            }

            match = Consume();

            return true;
        }

        private AssignmentStatement ParseAssignment()
        {
            var identifier = Consume().Content;
            
            if(!Expect(TokenType.Assign, out Token token))
            {
                AppendError("Expecting '=' after identifier.", token);

                return null;
            }

            var expression = ParseExpression();

            return new AssignmentStatement(identifier, expression);
        }

        private void ConsumeWhiteSpace()
        {
            while(true)
            {
                var next = Peek();

                if (next.TokenType == TokenType.NewLine)
                {
                    Consume();
                }
                else if (next is WSToken)
                {
                    var ws = Consume() as WSToken;
                    currentIndentationLevel = ws.IndentationLevel;
                }
                else
                    break;
            }
        }

        private Token Peek(int distance = 0)
        {
            while(read.Count <= distance)
            {
                read.Add(lexer.NextToken());
            }

            return read[distance];
        }

        private Token Consume()
        {
            if(read.Count == 0)
                Peek();

            var token = read[0];
            read.RemoveAt(0);

            return token;
        }

        private Precedence GetPrecedence()
        {
            var peek = Peek();

            if(infixParselets.TryGetValue(peek.TokenType, out IInfixParselet parselet))
            {
                return parselet.Precedence;
            }

            return Precedence.None;
        }

        private void AppendError(string error, Token token)
        {
            errorSink.AppendLine($"Error on line {token.Line}:{token.Column} -> {error}");
            MoveToNextLine();
        }

        private void MoveToNextLine()
        {
            while(true)
            {
                var next = Peek();

                if (next.TokenType == TokenType.EOF)
                    break;

                if(next.TokenType != TokenType.NewLine)
                {
                    Consume();
                    continue;
                }
                
                break;
            }
        }
    }
}

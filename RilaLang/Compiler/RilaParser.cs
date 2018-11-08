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
        
        public Module Parse()
        {
            var statements = new List<AstNode>();
            ConsumeNewLines();

            try
            {
                var token = Peek();

                while (token.TokenType != TokenType.EOF)
                {
                    statements.Add(ParseStatement());

                    token = Peek();
                }
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

            return new Module(lexer.Filename, statements);
        }

        internal AstNode ParseStatement()
        {
            var token = Peek();
            AstNode node = null;

            switch (token.TokenType)
            {
                case TokenType.Identifier:
                    node = ParseAssignment();
                    break;
                case TokenType.If:
                    node = ParseIf();
                    break;
                default:
                    node = ParseExpression();
                    break;
            }

            return node;
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

        internal bool Expect(out Token match, params TokenType[] types)
        {
            var token = Peek();

            if (!types.Contains(token.TokenType))
            {
                foreach(var type in types)
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
            
            if(!Expect(out Token token, TokenType.Assign))
            {
                AppendError("Expecting '=' after identifier.", token);

                return null;
            }

            var expression = ParseExpression();

            ExpectNewLine();

            return new AssignmentStatement(identifier, expression);
        }

        private IfStatement ParseIf()
        {
            Consume();
            var startingIndent = currentIndentationLevel;

            var branches = new List<IfStatement.IfBranch>();
            BlockExpression @else = null;

            do
            {
                var condition = ParseExpression();
                ExpectNewLine();
                var block = ParseBlock();

                var branch = new IfStatement.IfBranch(condition, block);
                branches.Add(branch);
            }
            while (startingIndent == currentIndentationLevel && ConsumeIf(TokenType.ElseIf));

            if(startingIndent == currentIndentationLevel && ConsumeIf(TokenType.Else))
            {
                ExpectNewLine();
                var block = ParseBlock();

                @else = block;
            }

            return new IfStatement(branches, @else);
        }

        private BlockExpression ParseBlock()
        {
            var startingIndent = currentIndentationLevel;
            var statements = new List<AstNode>();

            if (!Expect(out Token ws, TokenType.WhiteSpace) ||
                currentIndentationLevel >= (ws as WSToken).IndentationLevel)
            {
                AppendError($"Expecting an indented block.", ws);
            }
            else
                currentIndentationLevel = (ws as WSToken).IndentationLevel;

            while(startingIndent < currentIndentationLevel)
            {
                statements.Add(ParseStatement());
                var next = Peek();

                if (next.TokenType != TokenType.WhiteSpace)
                    currentIndentationLevel = 0;
                else
                    CheckWs(next);
            }

            return new BlockExpression(statements);
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

        private bool ConsumeIf(TokenType type)
        {
            if(Peek().TokenType == type)
            {
                Consume();
                return true;
            }

            return false;
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

        private void ReportNewLineError(Token token)
        {
            AppendError("Expecting a new line", token);
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

        private void ExpectNewLine()
        {
            if (!Expect(out Token line, TokenType.NewLine, TokenType.EOF))
                AppendError("Expecting a new line.", line);
            else
                ConsumeNewLines();
        }

        private bool CheckWs(Token token)
        {
            if(token is WSToken)
            {
                currentIndentationLevel = (token as WSToken).IndentationLevel;
                Consume();

                return true;
            }

            return false;
        }

        private void ConsumeNewLines()
        {
            while (true)
            {
                if (!ConsumeIf(TokenType.NewLine))
                    break;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RilaLang.Compiler.Ast;
using RilaLang.Compiler.Parselets;

namespace RilaLang.Compiler
{
    public partial class RilaParser
    {
        private Lexer lexer;
        private List<Token> read;
        private StringBuilder errorSink;

        private uint currentIndentationLevel;

        public RilaParser(Lexer lexer)
        {
            ConfigureExpressionRules();

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
                case TokenType.If:
                    node = ParseIf();
                    break;
                case TokenType.For:
                    node = ParseForLoop();
                    break;
                case TokenType.While:
                    node = ParseWhileLoop();
                    break;
                case TokenType.Continue:
                    node = ParseContinueStatement();
                    break;
                case TokenType.Break:
                    node = ParseBreakStatement();
                    break;
                case TokenType.Function:
                    node = ParseFunctionDefinition();
                    break;
                case TokenType.Return:
                    node = ParseReturnStatement();
                    break;
                case TokenType.Use:
                    node = ParseUseStatement();
                    break;
                default:
                    node = ParseExpressionOrAssignment();
                    ExpectNewLine();
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
                else if (IsAssignmentOperator(token.TokenType))
                    break;
                else
                    throw new RilaParserException($"Unrecognised token: {token.Content}");
            }

            return left;
        }

        internal Token Peek(int distance = 0)
        {
            while (read.Count <= distance)
            {
                read.Add(lexer.NextToken());
            }

            return read[distance];
        }

        internal Token Consume()
        {
            if (read.Count == 0)
                Peek();

            var token = read[0];
            read.RemoveAt(0);

            return token;
        }

        internal bool ConsumeIf(TokenType type)
        {
            if (Peek().TokenType == type)
            {
                Consume();
                return true;
            }

            return false;
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

        internal void AppendError(string error, Token token)
        {
            errorSink.AppendLine($"Error on line {token.Line}:{token.Column} -> {error}");
        }

        private AstNode ParseExpressionOrAssignment()
        {
            var left = ParseExpression();
            var token = Peek();

            if (token.TokenType == TokenType.NewLine || token.TokenType == TokenType.EOF)
                return left;

            return FinishAssignment(left);
        }

        private AssignmentStatement FinishAssignment(Expression left)
        {
            var operation = Consume().TokenType;
            var right = ParseExpression();

            return new AssignmentStatement(left, operation, right);
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
                var block = ParseBlock();

                var branch = new IfStatement.IfBranch(condition, block);
                branches.Add(branch);
            }
            while (startingIndent == currentIndentationLevel && ConsumeIf(TokenType.ElseIf));

            if(startingIndent == currentIndentationLevel && ConsumeIf(TokenType.Else))
            {
                var block = ParseBlock();
                @else = block;
            }

            return new IfStatement(branches, @else);
        }

        private FunctionDefinition ParseFunctionDefinition()
        {
            Consume(); // fun

            Expect(out Token name, TokenType.Identifier);
            Expect(out Token _, TokenType.LParen);

            var args = new List<IdentifierExpression>();

            if (Peek().TokenType != TokenType.RParen)
            {
                do
                {
                    var next = Peek();
                    if(next.TokenType != TokenType.Identifier)
                    {
                        Consume();
                        AppendError($"Expecting argument name, got: {next.Content}", next);

                        continue;
                    }

                    args.Add(ParseExpression() as IdentifierExpression);

                    if (Peek().TokenType == TokenType.RParen || (!Expect(out Token _, TokenType.Comma)))
                        break;
                }
                while (true);
            }

            Expect(out Token _, TokenType.RParen);

            return new FunctionDefinition(name?.Content, args, ParseBlock());
        }

        private ReturnStatement ParseReturnStatement()
        {
            Consume(); // return

            Expression expression = null;

            if (Peek().TokenType != TokenType.NewLine) // void return
                expression = ParseExpression();

            ExpectNewLine();

            return new ReturnStatement(expression);
        }

        private BlockExpression ParseBlock()
        {
            if (!PrepareForBlock())
                return null;

            var startingIndent = currentIndentationLevel;
            var statements = new List<AstNode>();
            
            while(startingIndent == currentIndentationLevel)
            {
                statements.Add(ParseStatement());
                var next = Peek();

                currentIndentationLevel = next.IndentationLevel;
            }

            return new BlockExpression(statements);
        }

        private ForLoopStatement ParseForLoop()
        {
            Consume(); // for

            Expect(out Token identifier, TokenType.Identifier);
            Expect(out Token _, TokenType.In);

            var inExpression = ParseExpression();
            var block = ParseBlock();

            return new ForLoopStatement(identifier?.Content, inExpression, block);
        }

        private WhileLoopStatement ParseWhileLoop()
        {
            Consume(); // while

            var condition = ParseExpression();
            var block = ParseBlock();

            return new WhileLoopStatement(condition, block);
        }

        private ContinueStatement ParseContinueStatement()
        {
            Consume(); // continue
            ExpectNewLine();
            return new ContinueStatement();
        }

        private BreakStatement ParseBreakStatement()
        {
            Consume(); // break
            ExpectNewLine();
            return new BreakStatement();
        }

        private UseStatement ParseUseStatement()
        {
            Consume(); //use

            var token = Peek();
            var builder = new StringBuilder();

            while(token.TokenType != TokenType.NewLine &&
                  token.TokenType != TokenType.As && 
                  token.TokenType != TokenType.EOF)
            {
                builder.Append(Consume().Content);
                token = Peek();
            }

            if (builder.Length == 0)
                AppendError("Expecting namespace!", token);

            string alias = string.Empty; //Shouldn't be null

            if(ConsumeIf(TokenType.As))
            {
                token = Consume();

                if (token.TokenType != TokenType.Identifier)
                    AppendError("Expecting alias identifier!", token);

                alias = token.Content;
            }

            ExpectNewLine();

            return new UseStatement(builder.ToString(), alias);
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

        private bool IsAssignmentOperator(TokenType type)
        {
            //Put other syntactic sugar operators
            return type == TokenType.Assign;
        }

        private void ReportNewLineError(Token token)
        {
            AppendError("Expecting a new line", token);
            MoveToNextLine();
        }

        internal void MoveToNextLine()
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

        private bool ExpectNewLine()
        {
            var peek = Peek();

            if (peek.TokenType == TokenType.NewLine || peek.TokenType == TokenType.EOF)
            {
                ConsumeNewLines();
                return true;
            }
            else
            {
                ReportNewLineError(peek);
                return false;
            }
        }

        private bool ExpectIndentation()
        {
            var peek = Peek();

            if (peek.TokenType == TokenType.EOF)
                return false;

            if (currentIndentationLevel > peek.IndentationLevel)
            {
                AppendError($"Expecting an indented block.", peek);

                return false;
            }
            else
            {
                currentIndentationLevel = peek.IndentationLevel;

                return true;
            }
        }

        private bool PrepareForBlock()
        {
            return ExpectNewLine() && ExpectIndentation();
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

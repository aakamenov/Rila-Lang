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

        public AstNode ParseStatement()
        {
            throw new NotImplementedException();
        }

        public Expression ParseExpression()
        {
            var token = Consume();
            Expression left = null;

            if (prefixParselets.TryGetValue(token.TokenType, out IPrefixParselet prefixParselet))
            {
                left = prefixParselet.Parse(this, token);
            }
            else
                throw new RilaParserException($"Unrecognised token: {token.Content}");

            token = Peek();

            if (!infixParselets.TryGetValue(token.TokenType, out IInfixParselet infixParselet))
            {
                return left;
            }

            Consume();

            return infixParselet.Parse(this, token, left);
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

        private Token Expect(TokenType type)
        {
            var token = Peek();

            if (token.TokenType != type)
                AppendError($"Expecting '{type.ToString()}', found {token.Content}", token);

            return Consume();
        }

        private void AppendError(string error, Token token)
        {
            errorSink.AppendLine($"Error on line {token.Line}:{token.Column} -> {error}");
        }
    }
}

using System;
using System.Collections.Generic;

namespace RilaLang.Compiler
{
    public class Token
    {
        public TokenType TokenType { get; }
        public string Content { get; }
        public uint Line { get; }
        public uint Column { get; }

        private static Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
        {
            { "if", TokenType.If },
            { "elif", TokenType.ElseIf },
            { "else", TokenType.Else },
            { "is", TokenType.Is },
            { "fun", TokenType.Function },
            { "int", TokenType.Int },
            { "string", TokenType.String },
            { "for", TokenType.For },
            { "in", TokenType.In },
            { "return", TokenType.Return },
            { "true", TokenType.True },
            { "false", TokenType.False },
            { "not", TokenType.Not },
            { "and", TokenType.And },
            { "or", TokenType.Or }
        };

        public Token(TokenType type, string content, uint line, uint column)
        {
            TokenType = type;
            Content = content;
            Line = line;
            Column = column;
        }

        public static bool TryGetKeyword(string word, out TokenType tokenType)
        {
            if(keywords.ContainsKey(word))
            {
                tokenType = keywords[word];
                return true;
            }

            tokenType = default(TokenType);
            return false;
        }

        public override string ToString()
        {
            var content = string.IsNullOrEmpty(Content) ? "None" : Content;
            return $"Token Type: {TokenType}, Content: {content}, Line: {Line}, Column: {Column}";
        }
    }

    public class WSToken : Token
    {
        public uint IndentationLevel { get; }

        public WSToken(uint indentationLevel, uint line, uint column) 
            : base(TokenType.WhiteSpace, string.Empty, line, column)
        {
            IndentationLevel = indentationLevel;
        }
    }


    public enum TokenType
    {
        None = 0,
        EOF,
        NewLine,
        WhiteSpace,

        Identifier,
        Int,
        Bool,
        String,

        StringLiteral,
        NumericLiteral,

        Assign,
        Plus,
        Minus,
        Not,
        And,
        Or,
        Asterisk,
        Slash,
        LessThan,
        EqLessThan,
        GreaterThan,
        EqGreaterThan,
        Equal,
        NotEqual,
        Dot,
        Range,
        Modulo,

        Comma,
        Semicolon,
        Arrow,
        LParen,
        RParen,
        
        Function,
        If,
        Else,
        ElseIf,
        Is,
        Use,
        For,
        In,
        Return,
        True,
        False
    }
}

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
        public uint IndentationLevel { get; }

        private static Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
        {
            { "if", TokenType.If },
            { "elif", TokenType.ElseIf },
            { "else", TokenType.Else },
            { "is", TokenType.Is },
            { "fun", TokenType.Function },
            { "for", TokenType.For },
            { "in", TokenType.In },
            { "while", TokenType.While },
            { "return", TokenType.Return },
            { "true", TokenType.True },
            { "false", TokenType.False },
            { "not", TokenType.Not },
            { "and", TokenType.And },
            { "or", TokenType.Or },
            { "break", TokenType.Break },
            { "continue", TokenType.Continue },
            { "new", TokenType.New },
            { "use", TokenType.Use },
            { "as", TokenType.As }
        };

        public Token(TokenType type, string content, uint indentationLevel, uint line, uint column)
        {
            TokenType = type;
            Content = content;
            IndentationLevel = indentationLevel;
            Line = line;
            Column = column;
        }

        public static bool TryGetKeyword(string word, out TokenType tokenType)
        {
            if(keywords.TryGetValue(word, out tokenType))
                return true;

            tokenType = default(TokenType);
            return false;
        }

        public override string ToString()
        {
            var content = string.IsNullOrEmpty(Content) ? "None" : Content;
            return $"Token Type: {TokenType}, Content: {content}, Indentation: {IndentationLevel}, Line: {Line}, Column: {Column}";
        }
    }

    public enum TokenType
    {
        None = 0,
        EOF,
        NewLine,
        WhiteSpace,

        Identifier,

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
        LSquare,
        RSquare,
        
        Function,
        If,
        Else,
        ElseIf,
        Is,
        Use,
        For,
        In,
        While,
        Return,
        True,
        False,
        Break,
        Continue,
        New,
        As
    }
}

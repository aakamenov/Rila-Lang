using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            { "is", TokenType.Is },
            { "fun", TokenType.Function },
            { "int", TokenType.Int },
            { "string", TokenType.String },
            { "for", TokenType.For },
            { "in", TokenType.In },
            { "return", TokenType.Return }
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
            return $"Token Type: {TokenType}, Content: {Content}, Line: {Line}, Column: {Column}";
        }
    }

    public enum TokenType
    {
        None = 0,
        EOF,
        NewLine,
        Tab,

        Identifier,
        Int,
        Bool,
        String,

        StringLiteral,
        NumericLiteral,

        Assign,
        Plus,
        Minus,
        Bang,
        Asterisk,
        Slash,
        LessThan,
        GreaterThan,
        Equal,
        NotEqual,
        Dot,
        Range,

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
        Return
    }
}

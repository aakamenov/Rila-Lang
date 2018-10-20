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
        public int Line { get; }
        public int Column { get; }

        public Token(TokenType type, string content, int line, int column)
        {
            TokenType = type;
            Content = content;
            Line = line;
            Column = column;
        }
    }

    public enum TokenType
    {
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

        Comma,
        Semicolon,
        Arrow,
        
        Function,
        If,
        Else,
        ElseIf,
        Is,
        Use
    }
}

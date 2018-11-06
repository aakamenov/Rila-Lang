namespace RilaLang.Compiler.Parselets
{
    public enum Precedence
    {
        None = 0,
        Assign = 1,
        Or = 2,
        And = 3,
        Not = 4,
        Equality = 5,
        Sum = 6,
        Product = 7,
        Prefix = 8,
        Postfix = 9,
        Parentheses = 10
    }
}

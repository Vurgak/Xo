namespace Xo.Parsing;

public enum TokenKind
{
    NewLine,

    Identifier,
    Integer,

    Let,
    Print,

    Plus,
    Minus,
    Asterisk,
    Slash,
    Equals,

    Invalid,
    EndOfFile
}

namespace Xo.Parsing;

public enum TokenKind
{
    NewLine,

    Identifier,
    Integer,

    Let,

    Plus,
    Minus,
    Asterisk,
    Slash,
    Equals,

    Invalid,
    EndOfFile
}

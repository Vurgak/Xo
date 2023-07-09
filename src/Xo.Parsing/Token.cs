namespace Xo.Parsing;

public readonly record struct Token(TokenKind Kind, int Start, int End);

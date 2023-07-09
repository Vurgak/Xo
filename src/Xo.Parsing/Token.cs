using Xo.SourceCode;

namespace Xo.Parsing;

public readonly record struct Token(TokenKind Kind, SourceSpan Span);

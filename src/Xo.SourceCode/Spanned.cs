namespace Xo.SourceCode;

public readonly record struct Spanned<T>(SourceSpan Span, T Element);

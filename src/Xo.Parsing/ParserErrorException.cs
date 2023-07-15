﻿using Xo.SourceCode;

namespace Xo.Parsing;

internal class ParserErrorException : Exception
{
    public SourceSpan Span { get; }
    public string Message { get; }

    public ParserErrorException(SourceSpan span, string message)
    {
        Span = span;
        Message = message;
    }
}

using Xo.SourceCode;

namespace Xo.Parsing;

internal class ParserErrorException : Exception
{
    public SourceSpan Span { get; }

    public ParserErrorException(SourceSpan span, string message) : base(message)
    {
        Span = span;
    }
}

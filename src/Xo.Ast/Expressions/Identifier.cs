using Xo.Session;
using Xo.SourceCode;

namespace Xo.Ast.Expressions;

public sealed class Identifier : IExpression
{
    public Identifier(SourceSpan span, Symbol symbol)
    {
        Span = span;
        Symbol = symbol;
    }

    public SourceSpan Span { get; init; }
    public Symbol Symbol { get; }
}

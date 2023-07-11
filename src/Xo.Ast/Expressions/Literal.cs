using Xo.Session;
using Xo.SourceCode;

namespace Xo.Ast.Expressions;

public class Literal : IExpression
{
    public Literal(SourceSpan span, LiteralKind kind, Symbol symbol)
    {
        Span = span;
        Kind = kind;
        Symbol = symbol;
    }

    public SourceSpan Span { get; init; }
    public LiteralKind Kind { get; }
    public Symbol Symbol { get; }
}

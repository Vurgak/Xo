using Xo.SourceCode;

namespace Xo.Ast.Expressions;

public class Literal : IExpression
{
    public Literal(SourceSpan span, LiteralKind kind)
    {
        Span = span;
        Kind = kind;
    }

    public SourceSpan Span { get; set; }
    public LiteralKind Kind { get; set; }
}

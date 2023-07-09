using Xo.SourceCode;

namespace Xo.Ast;

public interface IAstNode
{
    public SourceSpan Span { get; set; }
}

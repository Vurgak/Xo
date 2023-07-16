using Xo.Ast.Expressions;
using Xo.SourceCode;

namespace Xo.Ast.Statements;

public class PrintStatement : IStatement
{
    public SourceSpan Span { get; init; }
    public IExpression Expression { get; }

    public PrintStatement(SourceSpan span, IExpression expression)
    {
        Span = span;
        Expression = expression;
    }
}

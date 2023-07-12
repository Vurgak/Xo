using Xo.Ast.Expressions;
using Xo.SourceCode;

namespace Xo.Ast.Statements;

public class ExpressionStatement : IStatement
{
    public required SourceSpan Span { get; init; }
    public required IExpression Expression { get; init; }
}

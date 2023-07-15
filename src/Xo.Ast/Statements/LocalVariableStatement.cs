using Xo.Ast.Expressions;
using Xo.SourceCode;

namespace Xo.Ast.Statements;

public sealed class LocalVariableStatement : IStatement
{
    public required SourceSpan Span { get; init; }
    public required Identifier Identifier { get; init; }
    public required IExpression Initializer { get; init; }
}

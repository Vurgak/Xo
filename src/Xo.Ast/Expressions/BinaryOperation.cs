using Xo.SourceCode;

namespace Xo.Ast.Expressions;

public class BinaryOperation : IExpression
{
    public required SourceSpan Span { get; init; }
    public required IExpression LeftOperand { get; init; }
    public required IExpression RightOperand { get; init; }
    public required Spanned<BinaryOperatorKind> Operator { get; init; }
}

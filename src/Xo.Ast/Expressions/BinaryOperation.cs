using Xo.SourceCode;

namespace Xo.Ast.Expressions;

public class BinaryOperation : IExpression
{
    public SourceSpan Span { get; set; }
    public IExpression LeftOperand { get; set; }
    public IExpression RightOperand { get; set; }
    public Spanned<BinaryOperatorKind> Operator { get; set; }
}

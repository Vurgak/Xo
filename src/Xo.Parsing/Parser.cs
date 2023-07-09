using Xo.Ast;
using Xo.Ast.Expressions;
using Xo.SourceCode;

namespace Xo.Parsing;

public class Parser
{
    private readonly TokenStream _tokens;

    private Parser(TokenStream tokens)
    {
        _tokens = tokens;
    }

    public static IAstNode Parse(TokenStream tokens)
    {
        var parser = new Parser(tokens);
        return parser.Parse();
    }

    private IAstNode Parse()
    {
        return ParseExpression();
    }

    private IExpression ParseExpression()
    {
        return ParseBinaryOperation();
    }

    #region Binary operation parsing

    private IExpression ParseBinaryOperation()
    {
        return ParseTermOperation();
    }

    private IExpression ParseTermOperation()
    {
        var expression = ParseFactorOperation();

        while (_tokens.Consume(new[] { TokenKind.Asterisk, TokenKind.Slash }, out var operatorToken))
        {
            var rightOperand = ParseFactorOperation();
            expression = new BinaryOperation
            {
                Span = new SourceSpan(expression.Span.Start, rightOperand.Span.End),
                LeftOperand = expression,
                RightOperand = rightOperand,
                Operator = MapTokenToBinaryOperator(operatorToken)
            };
        }

        return expression;
    }

    private IExpression ParseFactorOperation()
    {
        var expression = ParseLiteral();

        while (_tokens.Consume(new[] { TokenKind.Plus, TokenKind.Minus }, out var operatorToken))
        {
            var rightOperand = ParseFactorOperation();
            expression = new BinaryOperation
            {
                Span = new SourceSpan(expression.Span.Start, rightOperand.Span.End),
                LeftOperand = expression,
                RightOperand = rightOperand,
                Operator = MapTokenToBinaryOperator(operatorToken)
            };
        }

        return expression;
    }

    private static Spanned<BinaryOperatorKind> MapTokenToBinaryOperator(Token operatorToken) => operatorToken.Kind switch
    {
        TokenKind.Plus => new Spanned<BinaryOperatorKind>(operatorToken.Span, BinaryOperatorKind.Add),
        TokenKind.Minus => new Spanned<BinaryOperatorKind>(operatorToken.Span, BinaryOperatorKind.Subtract),
        TokenKind.Asterisk => new Spanned<BinaryOperatorKind>(operatorToken.Span, BinaryOperatorKind.Multiply),
        TokenKind.Slash => new Spanned<BinaryOperatorKind>(operatorToken.Span, BinaryOperatorKind.Divide),
        _ => throw new ArgumentOutOfRangeException(nameof(operatorToken), operatorToken,
            "Token is not a valid binary operator")
    };

    #endregion

    private IExpression ParseLiteral()
    {
        if (_tokens.Consume(TokenKind.Integer, out var integer))
            return new Literal(integer.Span, LiteralKind.Integer);

        throw new Exception($"{_tokens.Peek().Span.Start}: expected an expression");
    }
}

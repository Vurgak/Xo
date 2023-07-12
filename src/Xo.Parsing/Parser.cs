using Xo.Ast.Expressions;
using Xo.Ast.Statements;
using Xo.Session;
using Xo.SourceCode;

namespace Xo.Parsing;

public class Parser
{
    private readonly TokenStream _tokens;
    private readonly CompilationSession _session;

    private Parser(TokenStream tokens, CompilationSession session)
    {
        _tokens = tokens;
        _session = session;
    }

    public static IEnumerable<IStatement> Parse(TokenStream tokens, CompilationSession session)
    {
        var parser = new Parser(tokens, session);
        return parser.Parse();
    }

    private IEnumerable<IStatement> Parse()
    {
        var statements = new List<IStatement>();
        while (!_tokens.IsAtEnd())
            statements.Add(ParseStatement());

        return statements;
    }

    private IStatement ParseStatement()
    {
        var expression = ParseExpression();

        if (!_tokens.Consume(TokenKind.NewLine, out _))
            throw new Exception($"{_tokens.Peek().Span.Start}: expected a statement end");

        return new ExpressionStatement
        {
            Span = expression.Span,
            Expression = expression,
        };
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
        {
            var lexeme = _session.SourceFile.ReadSpan(integer.Span);
            var symbol = _session.SymbolCollection.Add(lexeme.ToString());
            return new Literal(integer.Span, LiteralKind.Integer, symbol);
        }

        throw new Exception($"{_tokens.Peek().Span.Start}: expected an expression");
    }
}

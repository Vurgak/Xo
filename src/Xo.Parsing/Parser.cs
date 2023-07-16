using Xo.Ast.Expressions;
using Xo.Ast.Statements;
using Xo.Session;
using Xo.SourceCode;

namespace Xo.Parsing;

public class Parser
{
    private readonly TokenStream _tokens;
    private readonly CompilationSession _session;

    private readonly IEnumerable<TokenKind> _literalTokenKinds = new[]
    {
        TokenKind.Identifier,
        TokenKind.Integer,
    };

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
        return ParseBlock();
    }

    private List<IStatement> ParseBlock()
    {
        var statements = new List<IStatement>();
        while (!_tokens.IsAtEnd())
        {
            try
            {
                statements.Add(ParseStatement());
                if (_tokens.Consume(TokenKind.NewLine, out _))
                    continue;

                var currentToken = _tokens.Peek();
                throw new ParserErrorException(currentToken.Span, "expected an operator or statement finisher");
            }
            catch (ParserErrorException exception)
            {
                _session.Diagnostics.EmitError(exception.Span, exception.Message);
                Synchronize();
            }
        }

        return statements;
    }

    private void Synchronize()
    {
        while (!_tokens.IsAtEnd() && !_tokens.Consume(TokenKind.NewLine, out _))
            _tokens.Advance();
    }

    private IStatement ParseStatement()
    {
        if (_tokens.Consume(TokenKind.Let, out var letToken))
            return ParseLocalVariableStatement(letToken);

        if (_tokens.Consume(TokenKind.Print, out var printToken))
            return ParsePrintStatement(printToken);

        return ParseExpressionStatement();
    }

    private IStatement ParseLocalVariableStatement(Token keywordToken)
    {
        var identifier = ParseIdentifier();

        if (!_tokens.Consume(TokenKind.Equals, out _))
        {
            var currentToken = _tokens.Peek();
            throw new ParserErrorException(currentToken.Span, "expected `='");
        }

        var initializer = ParseExpression();

        var span = new SourceSpan(keywordToken.Span.Start, initializer.Span.End);
        return new LocalVariableStatement
        {
            Span = span,
            Identifier = identifier,
            Initializer = initializer
        };
    }

    private IStatement ParsePrintStatement(Token printToken)
    {
        var expression = ParseExpression();
        var span = new SourceSpan(printToken.Span.Start, expression.Span.End);
        return new PrintStatement(span, expression);
    }

    private IStatement ParseExpressionStatement()
    {
        var expression = ParseExpression();
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
        _ => throw new ParserErrorException(operatorToken.Span, "expected a binary operator")
    };

    #endregion

    private Identifier ParseIdentifier()
    {
        if (_tokens.Consume(TokenKind.Identifier, out var token))
        {
            var symbol = AddSymbol(token);
            return new Identifier(token.Span, symbol);
        }

        var currentToken = _tokens.Peek();
        throw new ParserErrorException(currentToken.Span, "expected an identifier");
    }

    private IExpression ParseLiteral()
    {
        if (_tokens.Consume(_literalTokenKinds, out var literal))
            return NewLiteral(literal);

        var currentToken = _tokens.Peek();
        throw new ParserErrorException(currentToken.Span, "expected an expression");
    }

    private IExpression NewLiteral(Token literal)
    {
        var symbol = AddSymbol(literal);
        var literalKind = MapTokenKindToLiteralKind(literal.Kind);
        return new Literal(literal.Span, literalKind, symbol);
    }

    private Symbol AddSymbol(Token literal)
    {
        var lexeme = _session.SourceFile.ReadSpan(literal.Span);
        var symbol = _session.SymbolCollection.Add(lexeme.ToString());
        return symbol;
    }

    private static LiteralKind MapTokenKindToLiteralKind(TokenKind kind) => kind switch
    {
        TokenKind.Identifier => LiteralKind.Identifier,
        TokenKind.Integer => LiteralKind.Integer,
        _ => throw new ArgumentOutOfRangeException(nameof(kind))
    };
}

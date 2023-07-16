using Xo.Ast.Expressions;
using Xo.Ast.Statements;
using Xo.Session;

namespace Xo.AstAnalysis;

public class AstAnalyzer
{
    private readonly IReadOnlyList<IStatement> _ast;
    private readonly CompilationSession _session;

    private readonly Scope _rootScope = new();

    private AstAnalyzer(IReadOnlyList<IStatement> ast, CompilationSession session)
    {
        _ast = ast;
        _session = session;
    }

    public static void Analyze(IReadOnlyList<IStatement> ast, CompilationSession session)
    {
        var analyzer = new AstAnalyzer(ast, session);
        analyzer.Analyze();
    }

    private void Analyze()
    {
        foreach (var statement in _ast)
            AnalyzeStatement(statement);
    }

    private void AnalyzeStatement(IStatement statement)
    {
        switch (statement)
        {
            case LocalVariableStatement localVariable:
                AnalyzeLocalVariable(localVariable);
                break;

            case PrintStatement print:
                AnalyzePrintStatement(print);
                break;

            case ExpressionStatement expression:
                break;

            default:
                var statementTypeName = statement.GetType().Name;
                throw new ArgumentOutOfRangeException(nameof(statement), statementTypeName,
                    $"{nameof(AnalyzeStatement)} is not implemented for {statementTypeName}");
        }
    }

    private void AnalyzeLocalVariable(LocalVariableStatement localVariable)
    {
        AnalyzeExpression(localVariable.Initializer);

        if (!_rootScope.DefinitionExists(localVariable.Identifier.Symbol))
        {
            _rootScope.AddDefinition(localVariable.Identifier.Symbol);
            return;
        }

        var identifier = _session.SymbolCollection.Get(localVariable.Identifier.Symbol);
        _session.Diagnostics.EmitError(localVariable.Identifier.Span, $"symbol `{identifier}` is already defined in this scope");
    }

    private void AnalyzePrintStatement(PrintStatement print)
    {
        AnalyzeExpression(print.Expression);
    }

    private void AnalyzeExpression(IExpression expression)
    {
        switch (expression)
        {
            case BinaryOperation binaryOperation:
                AnalyzeBinaryOperation(binaryOperation);
                break;

            case Literal literal:
                AnalyzeLiteral(literal);
                break;

            default:
                var expressionTypeName = expression.GetType().Name;
                throw new ArgumentOutOfRangeException(nameof(expression), expressionTypeName,
                    $"{nameof(AnalyzeExpression)} is not implemented for {expressionTypeName}");
        }

    }

    private void AnalyzeBinaryOperation(BinaryOperation binaryOperation)
    {
        AnalyzeExpression(binaryOperation.LeftOperand);
        AnalyzeExpression(binaryOperation.RightOperand);
    }

    private void AnalyzeLiteral(Literal literal)
    {
        if (literal.Kind != LiteralKind.Identifier)
            return;

        if (_rootScope.DefinitionExists(literal.Symbol))
            return;

        var identifier = _session.SymbolCollection.Get(literal.Symbol);
        _session.Diagnostics.EmitError(literal.Span, $"symbol `{identifier}` is not defined in this scope");
    }
}

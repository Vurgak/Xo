using System.Reflection;
using System.Text;
using Xo.Ast.Expressions;
using Xo.Ast.Statements;
using Xo.Session;
using Xo.Tcc;

namespace Xo.CodeGeneration;

public class CodeGenerator
{
    private readonly IEnumerable<IStatement> _program;
    private readonly CompilationSession _session;
    private readonly StringBuilder _generatedCode = new();

    private CodeGenerator(IEnumerable<IStatement> program, CompilationSession session)
    {
        _program = program;
        _session = session;
    }

    public static void GenerateExecutable(string outputFilePath, IEnumerable<IStatement> program, CompilationSession session)
    {
        var generator = new CodeGenerator(program, session);
        generator.GenerateExecutable(outputFilePath);
    }

    private void GenerateExecutable(string outputFilePath)
    {
        GenerateCode();

        using var tcc = new TccState();
        tcc.CompileString(_generatedCode.ToString());
        tcc.OutputExecutable(outputFilePath);
    }

    private void GenerateCode()
    {
        string ReadEmbeddedResource(string resourceName)
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            var thisAssemblyName = thisAssembly.GetName().Name;
            var qualifiedResourceName = $"{thisAssemblyName}.{resourceName}";
            using var stream = thisAssembly.GetManifestResourceStream(qualifiedResourceName);
            using var reader = new StreamReader(stream!);
            return reader.ReadToEnd();
        }

        var prologue = ReadEmbeddedResource("Resources.GeneratedCodePrologue.c");
        _generatedCode.Append(prologue);

        foreach (var statement in _program)
            GenerateStatement(statement);

        var epilogue = ReadEmbeddedResource("Resources.GeneratedCodeEpilogue.c");
        _generatedCode.Append(epilogue);
    }

    private void GenerateStatement(IStatement statement)
    {
        switch (statement)
        {
            case ExpressionStatement expressionStatement:
                _generatedCode.Append("printf(\"");
                GenerateExpression(expressionStatement.Expression);
                _generatedCode.Append("\n\")");
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(statement), statement,
                    $"{nameof(GenerateStatement)} is not implemented for statement of type ${statement.GetType().Name}");
        }

        _generatedCode.Append(";\n");
    }

    private void GenerateExpression(IExpression expression)
    {
        switch (expression)
        {
            case BinaryOperation binaryOperation:
                GenerateBinaryOperation(binaryOperation);
                break;

            case Literal literal:
                GenerateLiteral(literal);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(expression), expression,
                    $"{nameof(GenerateExpression)} is not implemented for an expression of type ${expression.GetType().Name}");
        }
    }

    private void GenerateBinaryOperation(BinaryOperation binaryOperation)
    {
        GenerateExpression(binaryOperation.LeftOperand);
        _generatedCode.Append(' ');
        GenerateBinaryOperator(binaryOperation.Operator.Element);
        _generatedCode.Append(' ');
        GenerateExpression(binaryOperation.RightOperand);
    }

    private void GenerateBinaryOperator(BinaryOperatorKind binaryOperator)
    {
        switch (binaryOperator)
        {
            case BinaryOperatorKind.Add:
                _generatedCode.Append('+');
                break;

            case BinaryOperatorKind.Subtract:
                _generatedCode.Append('-');
                break;

            case BinaryOperatorKind.Multiply:
                _generatedCode.Append('*');
                break;

            case BinaryOperatorKind.Divide:
                _generatedCode.Append('/');
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(binaryOperator), binaryOperator,
                    $"{nameof(GenerateBinaryOperator)} is not implemented for an operator of type ${binaryOperator}");
        }
    }

    private void GenerateLiteral(Literal literal)
    {
        _generatedCode.Append(_session.SymbolCollection.Get(literal.Symbol));
    }
}

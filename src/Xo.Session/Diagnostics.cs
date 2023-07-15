using Spectre.Console;
using Xo.SourceCode;

namespace Xo.Session;

public class Diagnostics
{
    private readonly SourceFile _sourceFile;

    public Diagnostics(SourceFile sourceFile)
    {
        _sourceFile = sourceFile;

        AnsiConsole.ResetColors();
    }

    public int ErrorCount { get; private set; }

    public void EmitError(SourceSpan span, string message)
    {
        ErrorCount++;
        var lineNumber = _sourceFile.GetLineNumber(span.Start);
        var lineSpan = _sourceFile.ReadLineSpan(lineNumber);
        var line = _sourceFile.ReadSpan(lineSpan);
        var columnNumber = span.Start - lineSpan.Start + 1;
        var digitCountInLineNumber = (int)Math.Floor(Math.Log10(lineNumber)) + 1;
        var underlineOffset = digitCountInLineNumber + 3 + columnNumber - 1;

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[red bold]error:[/] {message}");
        AnsiConsole.MarkupLine($"  -> {_sourceFile.Path}:{lineNumber}:{columnNumber}");
        AnsiConsole.Markup($"{lineNumber} | {line}");
        DrawUnderline("red", underlineOffset, span.Length);
    }

    private void DrawUnderline(string color, int offset, int length)
    {
        var offsetChars = new string(' ', offset);
        var underlineChars = new string('^', length);
        AnsiConsole.MarkupLine($"{offsetChars}[red]{underlineChars}[/]");
    }
}

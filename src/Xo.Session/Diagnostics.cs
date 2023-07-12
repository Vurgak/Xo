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
        var lineSpan = _sourceFile.GetLineSpan(lineNumber);
        var line = _sourceFile.ReadSpan(lineSpan);

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[red bold]error:[/] {message}");
        AnsiConsole.MarkupLine($"  -> {_sourceFile.Path}:{lineNumber}");
        AnsiConsole.MarkupLine($"{lineNumber} | {line}");
    }
}

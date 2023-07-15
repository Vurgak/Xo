namespace Xo.SourceCode;

public class SourceFile
{
    public string? Path { get; }

    public string SourceCode { get; }

    private readonly List<int> _lineStarts;

    private SourceFile(string? path, string sourceCode)
    {
        Path = path;
        SourceCode = sourceCode;

        _lineStarts = new List<int> { 0 };
        for (var i = 0; i < sourceCode.Length; ++i)
        {
            if (sourceCode[i] == '\n')
                _lineStarts.Add(i + 1);
        }
    }

    public static SourceFile? ReadFromDisk(string filePath)
    {
        try
        {
            var sourceCode = File.ReadAllText(filePath);
            return new SourceFile(filePath, sourceCode);
        }
        catch
        {
            return null;
        }
    }

    public static SourceFile ReadFromString(string sourceCode)
    {
        return new SourceFile(null, sourceCode);
    }

    public ReadOnlySpan<char> ReadSpan(SourceSpan span) => SourceCode.AsSpan(span.Start, span.Length);

    public int GetLineNumber(int position) =>_lineStarts.FindIndex(lineStart => position < lineStart);

    public SourceSpan ReadLineSpan(int lineNumber) => new(_lineStarts[lineNumber - 1], _lineStarts[lineNumber]);
}

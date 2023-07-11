namespace Xo.SourceCode;

public class SourceFile
{
    public string SourceCode { get; }

    private SourceFile(string sourceCode)
    {
        SourceCode = sourceCode;
    }

    public static SourceFile? ReadFromDisk(string filePath)
    {
        try
        {
            var sourceCode = File.ReadAllText(filePath)
                .Replace("\r\n", "\n");

            return new SourceFile(sourceCode);
        }
        catch
        {
            return null;
        }
    }

    public ReadOnlySpan<char> ReadSpan(SourceSpan span) => SourceCode.AsSpan(span.Start, span.Length);
}

using Xo.SourceCode;

namespace Xo.Session;

public class CompilationSession
{
    public CompilationSession(SourceFile sourceFile)
    {
        Diagnostics = new Diagnostics(sourceFile);
        SourceFile = sourceFile;
    }

    public Diagnostics Diagnostics { get; }
    public SourceFile SourceFile { get; }
    public SymbolCollection SymbolCollection { get; } = new();
}

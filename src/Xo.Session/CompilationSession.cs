using Xo.SourceCode;

namespace Xo.Session;

public class CompilationSession
{
    public CompilationSession(SourceFile sourceFile)
    {
        SourceFile = sourceFile;
    }

    public SourceFile SourceFile { get; }
    public SymbolCollection SymbolCollection { get; } = new();
}

namespace Xo.Tcc;

public struct TccState : IDisposable
{
    private readonly IntPtr _ptr;

    public TccState()
    {
        _ptr = LibTcc.New();
        LibTcc.AddIncludePath(_ptr, ".");
        LibTcc.AddSysIncludePath(_ptr, ".");
        LibTcc.AddLibraryPath(_ptr, ".");
    }

    public void Dispose()
    {
        LibTcc.Delete(_ptr);
    }

    public readonly int CompileString(string source) => LibTcc.CompileString(_ptr, source);

    public readonly int OutputExecutable(string filePath)
    {
        LibTcc.SetOutputType(_ptr, (int)TccOutputType.Exe);
        return LibTcc.OutputFile(_ptr, filePath);
    }

}

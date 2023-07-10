using System.Runtime.InteropServices;

namespace Xo.Tcc;

internal static partial class LibTcc
{
    private const string _libraryName = "libtcc";

    [LibraryImport(_libraryName, EntryPoint = "tcc_new")]
    internal static partial IntPtr New();

    [LibraryImport(_libraryName, EntryPoint = "tcc_delete")]
    internal static partial void Delete(IntPtr state);

    [LibraryImport(_libraryName, EntryPoint = "tcc_compile_string", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int CompileString(IntPtr state, string buffer);

    [LibraryImport(_libraryName, EntryPoint = "tcc_output_file", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int OutputFile(IntPtr state, string pathName);

    [LibraryImport(_libraryName, EntryPoint = "tcc_set_output_type", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void SetOutputType(IntPtr state, int outputType);

    [LibraryImport(_libraryName, EntryPoint = "tcc_add_include_path", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int AddIncludePath(IntPtr source, string pathName);

    [LibraryImport(_libraryName, EntryPoint = "tcc_add_sysinclude_path", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int AddSysIncludePath(IntPtr state, string pathName);

    [LibraryImport(_libraryName, EntryPoint = "tcc_add_library_path", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int AddLibraryPath(IntPtr state, string pathName);

    [LibraryImport(_libraryName, EntryPoint = "tcc_add_library", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int AddLibrary(IntPtr state, string libraryName);
}

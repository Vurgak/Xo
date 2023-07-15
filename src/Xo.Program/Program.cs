using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using Xo.CodeGeneration;
using Xo.Parsing;
using Xo.Session;
using Xo.SourceCode;

var version = GetVersion();
Console.WriteLine($"xo {version}");

var stopwatch = new Stopwatch();
stopwatch.Start();

var sourceFilePath = args[0];

var sourceFile = SourceFile.ReadFromDisk(sourceFilePath);
if (sourceFile is null)
{
    Console.WriteLine($"error: couldn't open file \"{sourceFilePath}\"");
    Environment.Exit(1);
}

var session = new CompilationSession(sourceFile);
var tokens = new TokenStream(session);
var ast = Parser.Parse(tokens, session);
if (session.Diagnostics.ErrorCount > 0)
    TerminateCompilation(stopwatch, session.Diagnostics.ErrorCount);

var parsingTime = stopwatch.Elapsed.TotalSeconds;

var outputFileName = GetDefaultOutputFileName();
CodeGenerator.GenerateExecutable(outputFileName, ast, session);

var codeGenerationTime = stopwatch.Elapsed.TotalSeconds - parsingTime;

Console.WriteLine($"Compilation finished in {stopwatch.Elapsed.TotalSeconds:0.0000000}s");
Console.WriteLine($"  Parsing time: {parsingTime:0.0000000}s");
Console.WriteLine($"  Code generation time: {codeGenerationTime:0.0000000}s");

static string GetVersion()
{
    var assembly = Assembly.GetExecutingAssembly();
    var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
    var version = versionInfo.ProductVersion!;
    return version;
}

static void TerminateCompilation(Stopwatch compilationStopwatch, int errorCount)
{
    Console.WriteLine();
    Console.WriteLine($"Compilation finished in {compilationStopwatch.Elapsed.TotalSeconds}s");

    var errorsS = errorCount == 1 ? "" : "s";
    Console.WriteLine($"  with {errorCount} error{errorsS}");

    Environment.Exit(1);
}

static string GetDefaultOutputFileName() =>
    RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "program.exe" : "program";

using System.Diagnostics;
using System.Net.WebSockets;
using System.Reflection;
using System.Runtime.InteropServices;
using Xo.AstAnalysis;
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
var (ast, parsingTime) = RunTimedTransformation(() => Parser.Parse(tokens, session));
if (session.Diagnostics.ErrorCount > 0)
    TerminateCompilation(stopwatch, session.Diagnostics.ErrorCount);

var semanticAnalysisTime = RunTimedCompilationPass(() => AstAnalyzer.Analyze(ast, session));
if (session.Diagnostics.ErrorCount > 0)
    TerminateCompilation(stopwatch, session.Diagnostics.ErrorCount);

var outputFileName = GetDefaultOutputFileName();
var codeGenerationTime = RunTimedCompilationPass(() => CodeGenerator.GenerateExecutable(outputFileName, ast, session));

Console.WriteLine($"Compilation finished in {stopwatch.Elapsed.TotalSeconds:0.0000000}s");
Console.WriteLine($"  Parsing time: {parsingTime:0.0000000}s");
Console.WriteLine($"  Semantic analysis time: {semanticAnalysisTime:0.0000000}s");
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

static double RunTimedCompilationPass(Action pass)
{
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    pass();
    var elapsed = stopwatch.Elapsed.TotalSeconds;
    return elapsed;
}

static (TResult, double) RunTimedTransformation<TResult>(Func<TResult> pass)
{
    var stopwatch = new Stopwatch();
    stopwatch.Start();
    var result = pass();
    var elapsed = stopwatch.Elapsed.TotalSeconds;
    return (result, elapsed);
}
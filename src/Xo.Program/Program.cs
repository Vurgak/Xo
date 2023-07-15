using System.Diagnostics;
using System.Runtime.InteropServices;
using Xo.CodeGeneration;
using Xo.Parsing;
using Xo.Session;
using Xo.SourceCode;

const string version = "0.1.0-dev";
Console.WriteLine($"xo {version}");

var stopwatch = new Stopwatch();
stopwatch.Start();

var sourceFilePath = args[0];
Console.WriteLine(sourceFilePath);

var sourceFile = SourceFile.ReadFromDisk(sourceFilePath);
if (sourceFile is null)
{
    Console.WriteLine($"error: couldn't open file \"{sourceFilePath}\"");
    Environment.Exit(1);
}

var session = new CompilationSession(sourceFile);
var tokens = new TokenStream(session);
var ast = Parser.Parse(tokens, session);
var outputFileName = GetDefaultOutputFileName();
CodeGenerator.GenerateExecutable(outputFileName, ast, session);

Console.WriteLine($"Compilation finished in {stopwatch.Elapsed.TotalSeconds}s");

static string GetDefaultOutputFileName() =>
    RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "program.exe" : "program";

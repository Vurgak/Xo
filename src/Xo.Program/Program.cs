using System.Diagnostics;using System.Runtime.InteropServices;
using Xo.CodeGeneration;
using Xo.Parsing;
using Xo.SourceCode;

const string version = "0.1.0-dev";
Console.WriteLine($"xo {version}");

var stopwatch = new Stopwatch();
stopwatch.Start();

var sourceFilePath = args[0];
Console.WriteLine(sourceFilePath);

var sourceFile = SourceFile.ReadFromDisk(sourceFilePath);

var tokens = new TokenStream(sourceFile.SourceCode);

var ast = Parser.Parse(tokens);

var outputFileName = GetDefaultOutputFileName();
CodeGenerator.GenerateExecutable(outputFileName, ast, sourceFile);

Console.WriteLine($"Compilation finished in {stopwatch.Elapsed.TotalSeconds}s");

static string GetDefaultOutputFileName() =>
    RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "program.exe" : "program";

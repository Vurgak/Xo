using System.Diagnostics;
using Xo.Parsing;

const string version = "0.1.0-dev";
Console.WriteLine($"xo {version}");

var stopwatch = new Stopwatch();
stopwatch.Start();

var sourceFilePath = args[0];
Console.WriteLine(sourceFilePath);

var sourceCode = ReadSourceFile(sourceFilePath);

var tokens = new TokenStream(sourceCode);

var ast = Parser.Parse(tokens);

Console.WriteLine($"Compilation finished in {stopwatch.Elapsed.TotalSeconds}s");

// Automatically converts CRLF line endings to LF (for sanity).
static string ReadSourceFile(string filePath) =>
    File.ReadAllText(filePath)
        .Replace("\r\n", "\n");

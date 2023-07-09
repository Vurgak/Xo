using Xo.Parsing;

const string version = "0.1.0-dev";
Console.WriteLine($"xo {version}");

var sourceFilePath = args[0];
Console.WriteLine(sourceFilePath);

var sourceCode = ReadSourceFile(sourceFilePath);

var tokens = new TokenStream(sourceCode);

while (!tokens.IsAtEnd())
    Console.WriteLine(tokens.Advance().Kind);
Console.WriteLine(tokens.Advance().Kind);

// Automatically converts CRLF line endings to LF (for sanity).
static string ReadSourceFile(string filePath) =>
    File.ReadAllText(filePath)
        .Replace("\r\n", "\n");

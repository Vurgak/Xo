namespace Xo.SourceCode.UnitTests;

public class SourceFileTests
{
    [Theory]
    [InlineData("test\n", 0, 1)]
    [InlineData("test\n", 4, 1)]
    [InlineData("\n\ntest\n", 6, 3)]
    public void GetLineNumber_ShouldReturnCorrectLineNumberForPosition(string sourceCode, int position, int expectedLine)
    {
        var sourceFile = SourceFile.ReadFromString(sourceCode);

        var lineNumber = sourceFile.GetLineNumber(position);

        Assert.Equal(expectedLine, lineNumber);
    }
}

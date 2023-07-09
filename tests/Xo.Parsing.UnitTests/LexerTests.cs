namespace Xo.Parsing.UnitTests;

public class LexerTests
{
    [Theory]
    [InlineData("  2137", 2, 6)]
    [InlineData("2137  ", 0, 4)]
    [InlineData(" 2137 ", 1, 5)]
    public void ScanToken_ShouldIgnoreWhitespaceAroundTokenBeingScanned(string sourceCode, int expectedStart, int expectedEnd)
    {
        var lexer = new Lexer(sourceCode);

        var actualToken = lexer.NextToken();

        Assert.Equal(expectedStart, actualToken.Start);
        Assert.Equal(expectedEnd, actualToken.End);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("2147483647")]
    [InlineData("4294967295")]
    [InlineData("18446744073709551615")]
    public void ScanToken_ShouldTokenizeIntegerToken(string sourceCode)
    {
        var lexer = new Lexer(sourceCode);

        var actualToken = lexer.NextToken();

        Assert.Equal(TokenKind.Integer, actualToken.Kind);
    }

    [Theory]
    [InlineData("+", TokenKind.Plus)]
    [InlineData("-", TokenKind.Minus)]
    [InlineData("*", TokenKind.Asterisk)]
    [InlineData("/", TokenKind.Slash)]
    public void ScanToken_ShouldTokenizeSingleCharOperatorToken(string sourceCode, TokenKind expectedKind)
    {
        var lexer = new Lexer(sourceCode);

        var actualToken = lexer.NextToken();

        Assert.Equal(expectedKind, actualToken.Kind);
    }
}

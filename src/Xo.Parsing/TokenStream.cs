namespace Xo.Parsing;

public class TokenStream
{
    private readonly Lexer _lexer;

    /// <summary>
    /// A token cache is created when peeking a token and cleared when a token is advanced or consumed.
    /// </summary>
    private Token? _tokenCache;

    public TokenStream(string sourceCode)
    {
        _lexer = new Lexer(sourceCode);
    }

    public bool IsAtEnd()
    {
        var token = Peek();
        return token.Kind == TokenKind.EndOfFile;
    }

    public Token Peek()
    {
        _tokenCache ??= _lexer.NextToken();
        return _tokenCache.Value;
    }

    public Token Advance()
    {
        if (IsAtEnd())
            return _tokenCache!.Value;

        if (!_tokenCache.HasValue)
            return _lexer.NextToken();

        var token = _tokenCache.Value;
        _tokenCache = null;
        return token;
    }

    /// <summary>
    /// Advances the stream only if the next token is of given kind.
    /// </summary>
    public bool Consume(TokenKind expectedKind, out Token? token)
    {
        var nextToken = Peek();
        if (expectedKind != TokenKind.EndOfFile && nextToken.Kind == expectedKind)
        {
            token = nextToken;
            _lexer.NextToken();
            return true;
        }

        token = null;
        return false;
    }
}

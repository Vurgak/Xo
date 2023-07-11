using Xo.Session;
using Xo.SourceCode;

namespace Xo.Parsing;

public class TokenStream
{
    private readonly Lexer _lexer;

    /// <summary>
    /// A token cache is created when peeking a token and cleared when a token is advanced or consumed.
    /// </summary>
    private Token? _tokenCache;

    public TokenStream(CompilationSession session)
    {
        _lexer = new Lexer(session);
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
    /// Advances the stream only if the next token is of the given kind.
    /// </summary>
    public bool Consume(TokenKind expectedKind, out Token token)
    {
        var nextToken = Peek();
        if (expectedKind != TokenKind.EndOfFile && nextToken.Kind == expectedKind)
        {
            token = nextToken;
            _tokenCache = null;
            return true;
        }

        token = default;
        return false;
    }

    /// <summary>
    /// Advances the stream only if the next token is one of given kinds.
    /// </summary>
    public bool Consume(IEnumerable<TokenKind> expectedKinds, out Token token)
    {
        var nextToken = Peek();
        if (nextToken.Kind != TokenKind.EndOfFile && expectedKinds.Contains(nextToken.Kind))
        {
            token = nextToken;
            _tokenCache = null;
            return true;
        }

        token = default;
        return false;
    }
}

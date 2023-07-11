using Xo.Session;
using Xo.SourceCode;

namespace Xo.Parsing;

internal class Lexer
{
    private readonly string _sourceCode;
    private readonly Token _eofToken;

    private int _startPosition;
    private int _currentPosition;

    public Lexer(CompilationSession session)
    {
        _sourceCode = session.SourceFile.SourceCode;
        _eofToken = new Token(TokenKind.EndOfFile, new SourceSpan(_sourceCode.Length, _sourceCode.Length));
    }

    internal Lexer(string sourceCode)
    {
        _sourceCode = sourceCode;
        _eofToken = new Token(TokenKind.EndOfFile, new SourceSpan(_sourceCode.Length, _sourceCode.Length));
    }

    public Token NextToken()
    {
        if (_startPosition >= _sourceCode.Length)
            return _eofToken;

        var token = ScanToken();
        _startPosition = _currentPosition;
        return token;
    }

    private Token ScanToken()
    {
        var token = ScanWhiteSpaceToken();
        if (token.HasValue)
            return token.Value;

        var firstChar = PeekChar();

        if (IsNumberStart(firstChar))
            return ScanNumberToken();

        NextChar();
        return firstChar switch
        {
            '+' => NewToken(TokenKind.Plus),
            '-' => NewToken(TokenKind.Minus),
            '*' => NewToken(TokenKind.Asterisk),
            '/' => NewToken(TokenKind.Slash),
            _ => NewToken(TokenKind.Invalid)
        };
    }

    private Token? ScanWhiteSpaceToken()
    {
        SkipWhiteSpaceChars();
        if (IsAtEnd())
            return _eofToken;

        _startPosition = _currentPosition;
        return null;
    }

    private void SkipWhiteSpaceChars()
    {
        while (!IsAtEnd() && IsWhiteSpace(PeekChar()))
            NextChar();
    }

    private Token ScanNumberToken()
    {
        while (!IsAtEnd() && IsNumberStart(PeekChar()))
            NextChar();

        return NewToken(TokenKind.Integer);
    }

    private Token NewToken(TokenKind kind) => new(kind, new SourceSpan(_startPosition, _currentPosition));

    private bool IsAtEnd() => _currentPosition >= _sourceCode.Length;

    private char PeekChar() => _sourceCode[_currentPosition];

    private char NextChar() => _sourceCode[_currentPosition++];

    private static bool IsWhiteSpace(char c) => char.IsWhiteSpace(c);

    private static bool IsNumberStart(char c) => char.IsAsciiDigit(c);

    private static bool IsNumberContinuation(char c) => char.IsAsciiDigit(c) || c == '.';
}

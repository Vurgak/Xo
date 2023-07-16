using System.Collections.ObjectModel;
using Xo.Session;
using Xo.SourceCode;

namespace Xo.Parsing;

internal class Lexer
{
    private readonly string _sourceCode;
    private readonly Token _eofToken;

    private int _startPosition;
    private int _currentPosition;

    private static readonly ReadOnlyDictionary<string, TokenKind> _keywordKinds = new Dictionary<string, TokenKind>
    {
        { "let", TokenKind.Let},
        { "print", TokenKind.Print}
    }.AsReadOnly();

    public Lexer(CompilationSession session) : this(session.SourceFile.SourceCode)
    {
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

        if (IsIdentifierStart(firstChar))
            return ScanIdentifierOrKeywordToken();

        NextChar();
        return firstChar switch
        {
            '+' => NewToken(TokenKind.Plus),
            '-' => NewToken(TokenKind.Minus),
            '*' => NewToken(TokenKind.Asterisk),
            '/' => NewToken(TokenKind.Slash),
            '=' => NewToken(TokenKind.Equals),
            _ => NewToken(TokenKind.Invalid)
        };
    }

    private Token? ScanWhiteSpaceToken()
    {
        SkipInsignificantWhiteSpaceChars();
        while (!IsAtEnd() && PeekChar() == '\n')
        {
            NextChar();
            _startPosition = _currentPosition;

            SkipInsignificantWhiteSpaceChars();
            if (!IsAtEnd() && PeekChar() == '\n')
                continue;

            return NewToken(TokenKind.NewLine);
        }

        _startPosition = _currentPosition;
        return null;
    }

    private void SkipInsignificantWhiteSpaceChars()
    {
        while (!IsAtEnd() && IsInsignificantWhiteSpace(PeekChar()))
            NextChar();
    }

    private Token ScanNumberToken()
    {
        while (!IsAtEnd() && IsNumberStart(PeekChar()))
            NextChar();

        return NewToken(TokenKind.Integer);
    }

    private Token ScanIdentifierOrKeywordToken()
    {
        while (!IsAtEnd() && IsIdentifierContinuation(PeekChar()))
            NextChar();

        var lexemeLength = _currentPosition - _startPosition;
        var lexeme = _sourceCode.Substring(_startPosition, lexemeLength);
        var tokenKind = _keywordKinds.GetValueOrDefault(lexeme, TokenKind.Identifier);
        return NewToken(tokenKind);
    }

    private Token NewToken(TokenKind kind) => new(kind, new SourceSpan(_startPosition, _currentPosition));

    private bool IsAtEnd() => _currentPosition >= _sourceCode.Length;

    private char PeekChar() => _sourceCode[_currentPosition];

    private char NextChar() => _sourceCode[_currentPosition++];

    private static bool IsWhiteSpace(char c) => char.IsWhiteSpace(c);

    private static bool IsInsignificantWhiteSpace(char c) => char.IsWhiteSpace(c) && c != '\n';

    private static bool IsNumberStart(char c) => char.IsAsciiDigit(c);

    private static bool IsNumberContinuation(char c) => char.IsAsciiDigit(c) || c == '.';

    private static bool IsIdentifierStart(char c) => char.IsLetter(c) || c == '_';

    private static bool IsIdentifierContinuation(char c) => IsIdentifierStart(c) || char.IsDigit(c);
}

namespace Xo.Session;

public class SymbolCollection
{
    private readonly List<string> _symbols = new();

    public Symbol Add(string value)
    {
        var symbolIndex = _symbols.FindIndex(x => x == value);
        if (symbolIndex >= 0)
            return new Symbol(symbolIndex);

        var newSymbolIndex = _symbols.Count;
        _symbols.Add(value);
        return new Symbol(newSymbolIndex);
    }

    public string Get(Symbol symbol) => _symbols[symbol.Index];
}

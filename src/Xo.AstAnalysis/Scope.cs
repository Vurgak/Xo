using Xo.Session;

namespace Xo.AstAnalysis;

public class Scope
{
    private readonly HashSet<Symbol> _definitions = new();

    public void AddDefinition(Symbol symbol) => _definitions.Add(symbol);

    public bool DefinitionExists(Symbol symbol) => _definitions.Contains(symbol);
}

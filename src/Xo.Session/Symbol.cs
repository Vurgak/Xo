namespace Xo.Session;

public readonly struct Symbol
{
    internal Symbol(int index)
    {
        Index = index;
    }

    internal int Index { get; }
}
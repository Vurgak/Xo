﻿namespace Xo.SourceCode;

public readonly record struct SourceSpan(int Start, int End)
{
    public int Length => End - Start;
}

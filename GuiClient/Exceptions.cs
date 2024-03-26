using System;

namespace GuiClient;

public static class GuiExceptions
{
    public static Exception InvalidRepo(Type actual, Type expected)
    {
        return new InvalidOperationException(
            $"Unexpected repository. Expected {expected?.Name}, but was {actual?.Name}");
    }

    public static Exception InvalidFilter(string filter)
    {
        return new InvalidOperationException($"Unexpected filter '{filter}'");
    }
}
using System;
using System.Collections.Generic;

namespace GuiClient.Extensions;

public static class ListExtensions
{
    public static void Replace<TValue>(this IList<TValue> collection, TValue oldValue, TValue newValue)
    {
        ArgumentNullException.ThrowIfNull(collection);
        var index = collection.IndexOf(oldValue);
        collection[index] = newValue;
    }
}
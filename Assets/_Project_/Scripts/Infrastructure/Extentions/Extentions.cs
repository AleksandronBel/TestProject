using System.Collections.Generic;
using System;

public static class Extensions
{
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
        foreach (var item in items)
            action(item);

        return items;
    }
}

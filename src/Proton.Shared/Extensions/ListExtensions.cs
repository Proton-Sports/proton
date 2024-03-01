namespace System.Collections.Generic;

public static class ListExtensions
{
    public static T? ElementAtOrDefault<T>(this IReadOnlyList<T> list, int index)
    {
        return index > 0 && index < list.Count ? list[index] : default;
    }
}

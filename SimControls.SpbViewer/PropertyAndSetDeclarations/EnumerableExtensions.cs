using System;
using System.Collections.Generic;

namespace SimControls.SpbViewer.PropertyAndSetDeclarations;

internal static class EnumerableExtensions
{
    public static T[] ToSmallArray<T>(this IEnumerable<T> items) =>
        ToSmallArray(items.GetEnumerator(), 0);

    private static T[] ToSmallArray<T>(IEnumerator<T> enumerator, int length)
    {
        if (!enumerator.MoveNext()) return length == 0? Array.Empty<T>(): new T[length];
        var item = enumerator.Current;
        var ret = ToSmallArray(enumerator, length + 1);
        ret[length] = item;
        return ret;
    }
}
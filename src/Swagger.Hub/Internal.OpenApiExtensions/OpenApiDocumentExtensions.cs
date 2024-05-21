using System.Collections.Generic;
using System.Linq;

namespace GarageGroup.Infra;

internal static partial class OpenApiDocumentExtensions
{
    private const char Slash = '/';

    private static IEnumerable<T> Join<T>(this IEnumerable<T>? source, IEnumerable<T>? other)
    {
        if (other?.Any() is not true)
        {
            return source ?? [];
        }

        if (source?.Any() is not true)
        {
            return other;
        }

        var list = source.ToList();
        list.AddRange(other);

        return list;
    }

    private static Dictionary<string, T> ToDictionary<T>(this IEnumerable<KeyValuePair<string, T>> collection)
        =>
        collection.ToDictionary<Dictionary<string, T>, T>();

    private static TDictionary ToDictionary<TDictionary, T>(this IEnumerable<KeyValuePair<string, T>> collection)
        where TDictionary : IDictionary<string, T>, new()
    {
        var dictionary = new TDictionary();

        foreach (var item in collection)
        {
            dictionary[item.Key] = item.Value;
        }

        return dictionary;
    }
}
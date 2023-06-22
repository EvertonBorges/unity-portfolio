using System.Collections.Generic;
using System.Linq;

public static class ExtensionMethods
{

    private const string ResourcesFolder = "Assets/Resources";

    public static bool IsEmpty(this string value) => value == null || value == "" || value.Length <= 0 || value.ToLower() == "null";

    public static bool IsEmpty<T>(this T[] value) => value == null || value.Length <= 0;

    public static bool IsEmpty<T>(this IList<T> value) => value == null || value.Count <= 0;

    public static bool IsEmpty<T, U>(this Dictionary<T, U> value) => value == null || value.Count <= 0;

    public static string ToSingleString(this string[] values) => values.ToList().ToSingleString();

    public static string ToSingleString(this List<string> values)
    {
        string result = "";

        foreach (var value in values)
            result += $"{value}{(value == values[^1] ? "." : ", ")}";

        return result;
    }

}

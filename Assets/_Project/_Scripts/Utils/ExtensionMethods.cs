using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ExtensionMethods
{

    private const string ResourcesFolder = "Assets/Resources";

    public static bool IsEmpty(this string value) => value == null || value == "" || value.Length <= 0 || value.ToLower() == "null";

    public static bool IsEmpty<T>(this T[] value) => value == null || value.Length <= 0;

    public static bool IsEmpty<T>(this IList<T> value) => value == null || value.Count <= 0;

    public static bool IsEmpty<T, U>(this Dictionary<T, U> value) => value == null || value.Count <= 0;

    public static bool CompareTag(this Component self, Tags tag) => self.CompareTag(tag.ToString());

    public static string ToSingleString(this string[] values) => values.ToList().ToSingleString();

    public static void SetLayerRecursively(this GameObject self, LayerMask layer)
    {
        self.layer = layer;

        foreach (Transform child in self.transform)
            SetLayerRecursively(child.gameObject, layer);
    }

    public static void SetLayerRecursively(this GameObject self, string layer)
    {
        self.SetLayerRecursively(LayerMask.NameToLayer(layer));
    }

    public static string ToSingleString(this List<string> values)
    {
        string result = "";

        foreach (var value in values)
            result += $"{value}{(value == values[^1] ? "." : ", ")}";

        return result;
    }

    public static float Duration(this AnimationCurve self)
    {
        if (self.length > 0)
            return self[self.length - 1].time;

        return 0f;
    }

    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

    public static MazeNodeDirection InverseDirection(this MazeNodeDirection direction)
    {
        return direction switch
        {
            MazeNodeDirection.Up => MazeNodeDirection.Down,
            MazeNodeDirection.Right => MazeNodeDirection.Left,
            MazeNodeDirection.Down => MazeNodeDirection.Up,
            MazeNodeDirection.Left => MazeNodeDirection.Right,
            _ => MazeNodeDirection.Down,
        };
    }

}

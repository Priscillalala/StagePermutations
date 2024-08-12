using System.Runtime.CompilerServices;

namespace StagePermutations;

public static class SharedExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryFind(this Transform transform, string n, out Transform child)
    {
        return child = transform.Find(n);
    }

    public static IEnumerable<Transform> AllChildren(this Transform transform)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            yield return transform.GetChild(i);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddComponent<T>(this GameObject gameObject, out T component) where T : Component
    {
        component = gameObject.AddComponent<T>();
    }
}

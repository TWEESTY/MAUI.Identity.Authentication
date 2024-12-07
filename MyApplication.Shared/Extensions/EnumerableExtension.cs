namespace MyApplication.Shared.Extensions;

public static class EnumerableExtension
{
    public static IEnumerable<T> AsNotNull<T>(this IEnumerable<T> original)
    {
        return original ?? Enumerable.Empty<T>();
    }
}

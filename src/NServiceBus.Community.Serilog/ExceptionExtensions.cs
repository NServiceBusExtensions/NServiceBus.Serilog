#pragma warning disable 1591

static class ExceptionExtensions
{
    public static bool TryReadData<T>(this Exception exception, string key, out T state)
    {
        var data = exception.Data;
        if (data.Contains(key))
        {
            state = (T) data[key]!;
            data.Remove(key);
            return true;
        }

#pragma warning disable CS8653, CS8601
        state = default;
#pragma warning restore
        return false;
    }
}
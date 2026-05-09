using ProductCatalog.Application.Common.Interfaces;

namespace ProductCatalog.Infrastructure.Persistence;

/// <summary>Pure Dictionary-based search cache. IMemoryCache is explicitly not used per spec.</summary>
public sealed class SearchCacheRepository : ISearchCache
{
    private readonly Dictionary<string, object> _store = new();
    private readonly Lock _lock = new();

    public bool TryGet<T>(string key, out T? value)
    {
        lock (_lock)
        {
            if (_store.TryGetValue(key, out var raw) && raw is T typed)
            {
                value = typed;
                return true;
            }
            value = default;
            return false;
        }
    }

    public void Set<T>(string key, T value)
    {
        lock (_lock)
            _store[key] = value!;
    }

    public void Invalidate(string keyPrefix)
    {
        lock (_lock)
        {
            var keys = _store.Keys.Where(k => k.StartsWith(keyPrefix, StringComparison.Ordinal)).ToList();
            foreach (var key in keys)
                _store.Remove(key);
        }
    }
}

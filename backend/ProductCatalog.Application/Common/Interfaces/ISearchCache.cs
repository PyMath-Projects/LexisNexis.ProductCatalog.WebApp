namespace ProductCatalog.Application.Common.Interfaces;

public interface ISearchCache
{
    bool TryGet<T>(string key, out T? value);
    void Set<T>(string key, T value);
    void Invalidate(string keyPrefix);
}

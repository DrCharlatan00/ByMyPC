namespace ByMyPC.Caching
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task<long> GetVersionAsync(string key);
        Task<long> IncrementAsync(string key);
        Task SetAsync<T>(string key, T Value, TimeSpan time);
    }
}
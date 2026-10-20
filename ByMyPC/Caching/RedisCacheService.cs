using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace ByMyPC.Caching
{
    public class RedisCacheService(IDistributedCache cache, IConnectionMultiplexer redis) : ICacheService
    {
        private readonly IDistributedCache cache = cache;
        private readonly IConnectionMultiplexer redis = redis;


        public async Task<T?> GetAsync<T>(string key)
        {
            var json = await cache.GetAsync(key);

            if (json is null) return default;

            return JsonSerializer.Deserialize<T>(json);
        }

        public async Task SetAsync<T>(string key, T Value, TimeSpan time)
        {
            var json = JsonSerializer.Serialize(Value);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = time,
            };

            await cache.SetStringAsync(key, json, options);
        }


        public async Task<long> IncrementAsync(string key)
        {
            var database = redis.GetDatabase();
            return await database.StringIncrementAsync(key);
        }

        public async Task<long> GetVersionAsync(string key)
        {
            var database = redis.GetDatabase();
            var val = await database.StringGetAsync(key);
            return val.HasValue ? (long)val : 1;
        }
    }
}

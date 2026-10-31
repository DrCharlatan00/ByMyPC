namespace ByMyPC.Caching
{
    public class RedisCacheDataWork(ICacheService cacheService, ILogger<RedisCacheDataWork> logger)
    {
        private readonly ICacheService cacheService = cacheService;
        private readonly ILogger<RedisCacheDataWork> logger = logger;

        public async Task<T?> GetOrSetCacheAsync<T>(
        string cacheKey,
        Func<Task<T>> factory,
        TimeSpan expiration)
        {

#if DEBUG
            logger.LogInformation("Try get cache in redis");
#endif


            var cacheData = await cacheService.GetAsync<T>(cacheKey);
            if (cacheData is not null) return cacheData;

#if DEBUG
            logger.LogInformation("Cache not found get in factory");
#endif


            var cachedDataModels = await factory();
            if (cachedDataModels is not null)
            {

#if DEBUG
                logger.LogInformation("Caching");
#endif

                await cacheService.SetAsync(cacheKey, cachedDataModels, expiration);
            }

#if DEBUG
            logger.LogInformation("Exit cache Work");
#endif


            return cachedDataModels;

        }
    }
}

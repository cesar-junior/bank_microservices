using Microsoft.Extensions.Caching.Distributed;

namespace BankMicroservices.Client.Repository.Caching
{
    namespace RedisToDoList.API.Infrastructure.Caching
    {
        public class CachingService : ICachingService
        {
            private readonly IDistributedCache _cache;
            private readonly DistributedCacheEntryOptions _options;
            public CachingService(IDistributedCache cache)
            {
                _cache = cache;
                _options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20),
                    SlidingExpiration = TimeSpan.FromSeconds(5)
                };
            }

            public string? GetAsync(string key)
            {
                return _cache.GetString(key);
            }

            public async void SetAsync(string key, string value)
            {
                await _cache.SetStringAsync(key, value, _options);
            }
        }
    }
}

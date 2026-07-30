using E_Commerce.Application.Contracts;
using E_Commerce.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    internal class CacheService : ICacheService
    {   
        private readonly ICacheRepository _cacheRepository;

        public CacheService(ICacheRepository cacheRepository)
        {
            _cacheRepository = cacheRepository;
        }

        public ICacheRepository CacheRepository { get; }

        public async Task<string?> GetDataAsync(string cacheKey, CancellationToken cancellationToken = default)
            => await _cacheRepository.GetAsync(cacheKey, cancellationToken);

        public async Task SetDataAsync(string cacheKey, object cacheValue, TimeSpan? TimeToLive = null, CancellationToken cancellationToken = default)
        {
            var jsonValue = JsonSerializer.Serialize(cacheValue, new JsonSerializerOptions()
            {

                PropertyNamingPolicy = JsonNamingPolicy.CamelCase

            });
            await _cacheRepository.SetAsync(cacheKey, jsonValue, TimeToLive, cancellationToken);
        }
    }
}

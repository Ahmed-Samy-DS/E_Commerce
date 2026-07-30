using E_Commerce.Domain.Contracts;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace E_Commerce.Infrastructure.Repositories
{
    internal class CacheRepository : ICacheRepository
    {
        private readonly IDatabase _database;
        public CacheRepository(IConnectionMultiplexer connection)
        {
            _database = connection.GetDatabase();
        }
        public async Task<string> GetAsync(string cacheKey, CancellationToken cancellationToken = default)
        {
            var Value = await _database.StringGetAsync(cacheKey);   
            if(Value.IsNullOrEmpty)
            {
                return null;
            }
            else
            {
                return Value.ToString();
            }

            //return Value.IsNullOrEmpty ? null : Value.ToString();
        }
        public async Task SetAsync(string cacheKey, string cacheValue, TimeSpan? timeToLive = null, CancellationToken cancellationToken = default)
        {
            await _database.StringSetAsync(cacheKey, cacheValue, timeToLive ?? TimeSpan.FromDays(2));
        }
    }
}

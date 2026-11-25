using Application.Interfaces;
using Domain.Entities.BasketEntities;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _redis;
        public BasketRepository(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }
        public async Task<bool> DeleteBasketAsync(string basketId)
        {
            return await _redis.KeyDeleteAsync(basketId);
        }
        public async Task<CustomerBasket> GetBasketAsync(string basketId)
        {
            var data = await _redis.StringGetAsync(basketId);
            return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<CustomerBasket>(data);
        }
        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            var newValue = await _redis.StringSetAsync(basket.Id, JsonSerializer.Serialize(basket), TimeSpan.FromDays(1));
            if (!newValue) return null;
            return await GetBasketAsync(basket.Id);
        }
    }
}
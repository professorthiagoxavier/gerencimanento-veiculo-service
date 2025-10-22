using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Service
{
    public class CacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IConfiguration configuration, ILogger<CacheService> logger)
        {
            _logger = logger;
            
            var redisConnectionString = configuration.GetConnectionString("RedisConnection") 
                                      ?? "localhost:6379";
            
            try
            {
                _redis = ConnectionMultiplexer.Connect(redisConnectionString);
                _database = _redis.GetDatabase();
                _logger.LogInformation("Conexão com Redis estabelecida com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao conectar com Redis: {ConnectionString}", redisConnectionString);
                throw;
            }
        }

        public async Task<string?> GetAsync(string key)
        {
            try
            {
                var value = await _database.StringGetAsync(key);
                return value.HasValue ? value.ToString() : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar chave {Key} no Redis", key);
                return null;
            }
        }

        public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            try
            {
                await _database.StringSetAsync(key, value, expiry);
                _logger.LogDebug("Chave {Key} salva no Redis com sucesso", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar chave {Key} no Redis", key);
                throw;
            }
        }

        public async Task DeleteAsync(string key)
        {
            try
            {
                await _database.KeyDeleteAsync(key);
                _logger.LogDebug("Chave {Key} removida do Redis com sucesso", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover chave {Key} do Redis", key);
                throw;
            }
        }

        public async Task<bool> KeyExistsAsync(string key)
        {
            try
            {
                return await _database.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência da chave {Key} no Redis", key);
                return false;
            }
        }

        public async Task<bool> SetExpiryAsync(string key, TimeSpan expiry)
        {
            try
            {
                return await _database.KeyExpireAsync(key, expiry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao definir expiração da chave {Key} no Redis", key);
                return false;
            }
        }

        public void Dispose()
        {
            _redis?.Dispose();
        }
    }
}

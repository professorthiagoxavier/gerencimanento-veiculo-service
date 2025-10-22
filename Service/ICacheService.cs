namespace Service
{
    public interface ICacheService
    {
        Task<string?> GetAsync(string key);
        Task SetAsync(string key, string value, TimeSpan? expiry = null);
        Task DeleteAsync(string key);
        Task<bool> KeyExistsAsync(string key);
        Task<bool> SetExpiryAsync(string key, TimeSpan expiry);
    }
}

namespace BankMicroservices.Client.Repository.Caching
{
    public interface ICachingService
    {
        void SetAsync(string key, string value);
        string GetAsync(string key);
    }
}

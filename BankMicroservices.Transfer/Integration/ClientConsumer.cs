using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;
using System.Net.Sockets;
using Polly.RateLimit;
using System.Collections.Immutable;
using Polly.Retry;

namespace BankMicroservices.Transfer.Integration
{
    public class ClientConsumer : IClientConsumer
    {
        private static HttpClient httpClient = new()
        {
            BaseAddress = new Uri("https://localhost:7077"),
        };
        private readonly string token;

        static ImmutableArray<Type> networkExceptions = new[] { typeof(SocketException), typeof(HttpRequestException) }.ToImmutableArray();

        static ImmutableArray<Type> strategyExceptions = new[] { typeof(TimeoutRejectedException), typeof(BrokenCircuitException), typeof(RateLimitRejectedException), }.ToImmutableArray();

        static ImmutableArray<Type> retryableExceptions = networkExceptions
            .Union(strategyExceptions)
            .ToImmutableArray();

        private static readonly AsyncRetryPolicy RetryPolicy = Policy
                                .Handle<Exception>(ex => retryableExceptions.Contains(ex.GetType()))
                                .RetryAsync(3);

        public ClientConsumer(string token)
        {
            this.token = token;
        }

        public async Task<bool> UserHasBalance(string userId, float amount)
        {

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            await RetryPolicy.ExecuteAsync(async () =>
            {
                using HttpResponseMessage response = await httpClient.GetAsync($"User/UserHasBalance/{userId}/{amount}");

                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();

                return System.Boolean.Parse(jsonResponse);
            });

            return false;
        }

        public async Task TransferBalance(string senderUserId, string receiverUserId, float amount)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            using StringContent jsonContent = new(
                 JsonSerializer.Serialize(new
                 {
                     senderUserId,
                     receiverUserId,
                     quantity = amount
                 }),
                 Encoding.UTF8,
                 "application/json");
            await RetryPolicy.ExecuteAsync(async () =>
            {
                using HttpResponseMessage response = await httpClient.PatchAsync("User/TransferBalance", jsonContent);

                response.EnsureSuccessStatusCode();
            });
        }
    }
}

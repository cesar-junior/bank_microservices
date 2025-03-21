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
            BaseAddress = new Uri("https://localhost:7077/"),
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

            httpClient.DefaultRequestHeaders.Remove("Authorization");
            httpClient.DefaultRequestHeaders.Add("Authorization", token);
            var hasBalance = false;
            await RetryPolicy.ExecuteAsync(async () =>
            {
                using HttpResponseMessage response = await httpClient.GetAsync($"api/v1/User/UserHasBalance/{userId}/{amount}");

                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();

                hasBalance = System.Boolean.Parse(jsonResponse);
            });

            return hasBalance;
        }

        public async Task TransferBalance(string senderUserId, string receiverUserId, float amount)
        {
            httpClient.DefaultRequestHeaders.Remove("Authorization");
            httpClient.DefaultRequestHeaders.Add("Authorization", token);

            var parameters = new Dictionary<string, string>
                {
                    { "senderUserId", senderUserId },
                    { "receiverUserId", receiverUserId },
                    { "quantity", amount.ToString() }
                };

            var content = new FormUrlEncodedContent(parameters);


            // Create query string with parameters
            var queryString = new StringBuilder();
            queryString.Append("?senderUserId=").Append(Uri.EscapeDataString(senderUserId));
            queryString.Append("&receiverUserId=").Append(Uri.EscapeDataString(receiverUserId));
            queryString.Append("&quantity=").Append(Uri.EscapeDataString(amount.ToString()));

            await RetryPolicy.ExecuteAsync(async () =>
            {
                using HttpResponseMessage response = await httpClient.PatchAsync($"api/v1/User/TransferBalance/", content);

                response.EnsureSuccessStatusCode();
            });
        }
    }
}

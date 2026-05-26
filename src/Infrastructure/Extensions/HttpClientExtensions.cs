using System.Text;
using System.Text.Json;

namespace Infrastructure.Extensions
{
    public static class HttpClientExtensions
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static async Task<(HttpResponseMessage response, TObjectResult? objectSuccessResult)> SendRequestAsync<TObjectRequest, TObjectResult>(
            this HttpClient client,
            string endpoint,
            HttpMethod method,
            TObjectRequest? objectRequest,
            Action? setAuthorization = null,
            bool automaticParseResult = true,
            CancellationToken cancellationToken = default)
        {
            setAuthorization?.Invoke();

            var hasBody = objectRequest is not null;

            using var request = new HttpRequestMessage
            {
                Method = method,
                Content = hasBody
                    ? new StringContent(JsonSerializer.Serialize(objectRequest), Encoding.UTF8, "application/json")
                    : null,
                RequestUri = new Uri($"{client.BaseAddress}{endpoint}"),
            };

            var response = await client.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode || !automaticParseResult)
                return (response, default);

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var content = JsonSerializer.Deserialize<TObjectResult>(json, SerializerOptions);
            return (response, content);
        }
    }
}

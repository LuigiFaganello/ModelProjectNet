using System.Text;
using Newtonsoft.Json;

namespace Infrastructure.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<(HttpResponseMessage response, TObjectResult? objectSuccessResult)> SendRequestAsync<TObjectRequest, TObjectResult>(
            this HttpClient client,
            string endpoint,
            HttpMethod method,
            TObjectRequest? objectRequest,
            Action? setAuthorization = null,
            bool automaticParseResult = true)
        {
            setAuthorization?.Invoke();

            var hasBody = objectRequest is not null;

            var response = await client.SendAsync(new HttpRequestMessage
            {
                Method = method,
                Content = hasBody
                    ? new StringContent(JsonConvert.SerializeObject(objectRequest), Encoding.UTF8, "application/json")
                    : null,
                RequestUri = new Uri($"{client.BaseAddress}{endpoint}"),
            });

            if (!response.IsSuccessStatusCode || !automaticParseResult)
                return (response, default);

            var content = JsonConvert.DeserializeObject<TObjectResult>(await response.Content.ReadAsStringAsync());
            return (response, content);
        }
    }
}

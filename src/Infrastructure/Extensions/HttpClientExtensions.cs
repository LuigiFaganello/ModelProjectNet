using System.Text;
using Newtonsoft.Json;

namespace Infrastructure.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<(HttpResponseMessage response, TObjectResult objectSuccessResult)> SendRequestAsync<TObjectRequest, TObjectResult>(
            this HttpClient client, string endpoint, HttpMethod method, TObjectRequest objectRequest, Action SetAuthorization = null,
            bool automaticParseResult = true)
        {
            try
            {
                SetAuthorization?.Invoke();

                HttpResponseMessage response = null;

                bool content = true;
                if (objectRequest != null)
                    content = false;

                response = await client.SendAsync(new HttpRequestMessage
                {
                    Method = method,
                    Content = content ? null : new StringContent(JsonConvert.SerializeObject(objectRequest), Encoding.UTF8, "application/json"),
                    RequestUri = new Uri($"{client.BaseAddress}{endpoint}"),
                });

                if (!response.IsSuccessStatusCode)
                    return (response, default(TObjectResult));

                if (automaticParseResult)
                    return (response, JsonConvert.DeserializeObject<TObjectResult>(await response.Content.ReadAsStringAsync()));

                return (response, default(TObjectResult));

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void SetAuthorizationToken(this HttpClient httpClient, string token)
        {
            try
            {
                if (httpClient.DefaultRequestHeaders.Contains("Authorization"))
                    httpClient.DefaultRequestHeaders.Remove("Authorization");

                httpClient.DefaultRequestHeaders.Add("Authorization", token);
            }
            catch (Exception)
            {
            }
        }
    }
}

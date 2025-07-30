using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Configuration;
using Infrastructure.Extensions;
using Infrastructure.ExternalService.DTO;
using Infrastructure.ExternalService.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure.ExternalService
{
    public class ExampleService : IExampleService
    {
        private readonly ILogger<ExampleService> _logger;
        private readonly HttpClient _httpClient;
        private readonly AppSettings _settings;

        public ExampleService(ILogger<ExampleService> logger,
                            HttpClient httpClient,
                            IOptions<AppSettings> options)
        {
            _logger = logger;
            _httpClient = httpClient;
            _settings = options.Value;
            _httpClient.Timeout = TimeSpan.FromSeconds(_settings.Viacep.TimeOut);
            _httpClient.BaseAddress = new System.Uri(_settings.Viacep.BaseUrl);
        }

        public async Task<IEnumerable<ExampleServiceDTO>> GetCityByCountry(string country, string city)
        {
            try
            {
                var (response, content) = await _httpClient.SendRequestAsync<string, IEnumerable<ExampleServiceDTO>>(
                    $"ws/{country}/{city}/Paulista/json/", HttpMethod.Get, null, null);

                response.EnsureSuccessStatusCode();

                return content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todos os exemplos: {Message}", ex.Message);
                return new List<ExampleServiceDTO>() { };
            }
        }
    }
}

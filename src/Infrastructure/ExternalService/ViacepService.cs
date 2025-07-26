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
    public class ViacepService : IViacepService
    {
        private readonly ILogger<ViacepService> _logger;
        private readonly HttpClient _httpClient;
        private readonly AppSettings _settings;

        public ViacepService(ILogger<ViacepService> logger,
                            HttpClient httpClient,
                            IOptions<AppSettings> options)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(_settings.Viacep.TimeOut);
            _httpClient.BaseAddress = new System.Uri(_settings.Viacep.BaseUrl);
        }

        public async Task<IEnumerable<ViacepResultDTO>> GetCityByCountry(string country, string city)
        {
            try
            {
                var (response, content) = await _httpClient.SendRequestAsync<string, IEnumerable<ViacepResultDTO>>(
                    $"ws/{country}/{city}/Paulista/json/", HttpMethod.Get, null, null);

                response.EnsureSuccessStatusCode();

                return content;
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERRO AO ENVIAR MENSAGEM AO CANAL DE MENSAGEM. REQUEST: ERRO: {ex.Message}");
                return null;
            }
        }
    }
}

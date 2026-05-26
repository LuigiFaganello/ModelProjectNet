using Application.DTO;
using Application.Interfaces;
using Infrastructure.Extensions;
using Infrastructure.ExternalService.DTO;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ExternalService
{
    /// <summary>
    /// Adapter do ViaCEP para a porta <see cref="IExampleService"/> definida na camada de Aplicação.
    /// Traduz o DTO específico do provedor (<see cref="ExampleServiceDTO"/>) para o contrato
    /// neutro da aplicação (<see cref="AddressDto"/>).
    /// </summary>
    public class ExampleService : IExampleService
    {
        private readonly ILogger<ExampleService> _logger;
        private readonly HttpClient _httpClient;

        public ExampleService(ILogger<ExampleService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<AddressDto>> GetAddressesAsync(
            string state,
            string city,
            string street,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var endpoint = $"ws/{Uri.EscapeDataString(state)}/{Uri.EscapeDataString(city)}/{Uri.EscapeDataString(street)}/json/";

                var (response, content) = await _httpClient.SendRequestAsync<string, IEnumerable<ExampleServiceDTO>>(
                    endpoint, HttpMethod.Get, null, cancellationToken: cancellationToken);

                response.EnsureSuccessStatusCode();

                return (content ?? Enumerable.Empty<ExampleServiceDTO>())
                    .Select(MapToAddress)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter endereços do serviço externo: {Message}", ex.Message);
                return Enumerable.Empty<AddressDto>();
            }
        }

        private static AddressDto MapToAddress(ExampleServiceDTO dto) => new()
        {
            ZipCode = dto.Cep,
            Street = dto.Logradouro,
            Complement = dto.Complemento,
            Unit = dto.Unidade,
            Neighborhood = dto.Bairro,
            City = dto.Localidade, // 'localidade' do ViaCEP é a cidade
            State = dto.Uf,        // 'uf' é a sigla do estado (char(2))
        };
    }
}

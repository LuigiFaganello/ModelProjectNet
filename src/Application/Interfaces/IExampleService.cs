using Application.DTO;

namespace Application.Interfaces
{
    /// <summary>
    /// Porta (abstração) para o serviço de consulta de endereços externo.
    /// A implementação (adapter) vive na camada de Infraestrutura, respeitando a regra
    /// de dependência da Clean Architecture: Infraestrutura depende de Aplicação.
    /// </summary>
    public interface IExampleService
    {
        Task<IEnumerable<AddressDto>> GetAddressesAsync(
            string state,
            string city,
            string street,
            CancellationToken cancellationToken = default);
    }
}

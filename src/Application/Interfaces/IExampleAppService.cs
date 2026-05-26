using Application.DTO;
using Domain.Common;

namespace Application.Interfaces
{
    public interface IExampleAppService
    {
        Task<Result<IEnumerable<ExampleAppServiceDto>>> GetAll(CancellationToken cancellationToken);
        Task<Result<ExampleAppServiceDto>> GetByZipCode(string zipCode, CancellationToken cancellationToken);
        Task SyncCity(string state, string city, string street, CancellationToken cancellationToken);
    }
}

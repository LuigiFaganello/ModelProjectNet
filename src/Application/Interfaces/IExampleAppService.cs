using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;

namespace Application.Services
{
    public interface IExampleAppService
    {
        Task<IEnumerable<ExampleDto>> GetAll(CancellationToken cancellationToken);
        Task<ExampleDto> GetByZipCode(string zipCode, CancellationToken cancellationToken);
        Task SyncCity(CancellationToken cancellationToken);
    }
}

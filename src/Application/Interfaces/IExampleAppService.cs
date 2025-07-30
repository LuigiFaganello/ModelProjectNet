using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Domain.Common;

namespace Application.Services
{
    public interface IExampleAppService
    {
        Task<Result<IEnumerable<ExampleAppServiceDto>>> GetAll(CancellationToken cancellationToken);
        Task<Result<ExampleAppServiceDto>> GetByZipCode(string zipCode, CancellationToken cancellationToken);
        Task SyncCity(CancellationToken cancellationToken);
    }
}

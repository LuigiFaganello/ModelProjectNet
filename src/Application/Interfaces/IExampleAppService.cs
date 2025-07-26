using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IExampleAppService
    {
        Task<string> GetAll(CancellationToken cancellationToken);

        Task<string> GetByZipCode(string zipCode, CancellationToken cancellationToken);
    }
}

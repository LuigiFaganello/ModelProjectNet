using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.ExternalService.DTO;

namespace Infrastructure.ExternalService.Interface
{
    public interface IExampleService
    {
        Task<IEnumerable<ExampleResultDTO>> GetCityByCountry(string country, string city);
    }
}

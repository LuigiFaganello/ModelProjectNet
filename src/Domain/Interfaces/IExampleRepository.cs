using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IExampleRepository
    {
        Task<IEnumerable<Example>> GetAllAsync(CancellationToken cancellationToken);
        Task<Example> GetByZipCodeAsync(string zipCode, CancellationToken cancellationToken);
        Task<Example> AddAsync(Example entity);
        Task<Example> UpdateAsync(Example entity);
        Task<bool> DeleteAsync(int id);
    }
}

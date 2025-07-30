using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IExampleRepository : IRepositoryBase<Example>
    {
        Task<Example> GetByZipCodeAsync(string zipCode, CancellationToken cancellationToken);
    }
}

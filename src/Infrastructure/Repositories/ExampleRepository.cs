using System.Linq.Expressions;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ExampleRepository : RepositoryBase<Example>, IExampleRepository
    {
        public ExampleRepository(DataContext context) : base(context)
        {
        }
        public async Task<Example> GetByZipCodeAsync(string zipCode, CancellationToken cancellationToken)
        {
            return await _dbSet
                .Where(x => x.ZipCode == zipCode)
                .OrderByDescending(x => x.ZipCode)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
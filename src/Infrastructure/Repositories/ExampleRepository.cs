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
            if (string.IsNullOrWhiteSpace(zipCode))
                return null;

            var cleanZipCode = zipCode.Replace("-", "").Replace(".", "").Trim();

            return await _dbSet
                .Where(x => x.ZipCode == zipCode || x.ZipCode == cleanZipCode)
                .OrderByDescending(x => x.ZipCode)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
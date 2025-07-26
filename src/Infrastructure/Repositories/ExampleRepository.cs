using System.Linq.Expressions;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ExampleRepository : IExampleRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<Example> _dbSet;
        public ExampleRepository(DataContext context)
        {
            _context = context;
            _dbSet = context.Examples;
        }
        public async Task<IEnumerable<Example>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbSet
                .OrderBy(x => x.ZipCode)
                .ToListAsync(cancellationToken);
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

        public async Task<Example> AddAsync(Example entity, CancellationToken cancellationToken)
        {
            var entry = await _dbSet.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entry.Entity;
        }

        public async Task<Example> UpdateAsync(Example entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
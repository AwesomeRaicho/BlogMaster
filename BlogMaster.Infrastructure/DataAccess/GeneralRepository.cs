using Azure;
using BlogMaster.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Infrastructure.DataAccess
{
    public class GeneralRepository<T> : IRepository<T> where T : class
    {
        protected readonly EntityDbContext _context;

        public GeneralRepository(EntityDbContext entityDbContext)
        {
            _context = entityDbContext;
        }

        public async Task Create(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(T entity)
        {
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<T?> Get(Guid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task Update(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<T>> GetAll(int page, int pageSize)
        {
            return await _context.Set<T>().Skip((page -1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<T?> Find(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T?>> FindAll(Expression<Func<T, bool>> predicate, int pageIndex, int pageSize)
        {
            return await _context.Set<T>()
                .Where(predicate)
                .Skip((pageSize - 1) * pageIndex)
                .Take(pageSize)
                .ToListAsync();
        }


    }
}

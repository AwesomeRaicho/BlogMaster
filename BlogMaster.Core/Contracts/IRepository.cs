using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Contracts
{
    public interface IRepository<T> where T : class
    {
        public Task Create(T entity);
        public Task<T?> Get(Guid id);
        public Task Update(T entity);
        public Task Delete(T entity);

        public Task<bool> AddRangeAsync(List<T> Range);
        public Task<bool> RemoveRangeAsync(List<T> Range);


        public Task<IEnumerable<T>> GetAll(int pageIndex, int pageSize);

        /// <summary>
        /// Finds an entity with specific requirements, implement your own filter method from the service layer.
        /// </summary>
        /// <param name="predicate">Lambda expression</param>
        /// <returns> T? Entity </returns>
        public Task<T?> Find(Expression<Func<T, bool>> predicate);


        public Task<IEnumerable<T?>> FindAll( Expression<Func<T, bool>> predicate, int pageIndex, int pageSize);



    }
}

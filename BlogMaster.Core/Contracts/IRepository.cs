using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Contracts
{
    public interface IRepository<T> where T : class
    {
        public Task Create(T entity);
        public Task<T?> Get(Guid id);
        public Task Update(T entity);
        public Task Delete(Guid id);


        public Task<IEnumerable<T>> GetAll(int page, int pageSize);

    }
}

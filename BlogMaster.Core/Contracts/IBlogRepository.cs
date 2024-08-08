using BlogMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Contracts
{
    public interface IBlogRepository : IRepository<Blog>
    {
        public Task<Blog?> GetBlogBySlug(string slug);

        public Task<IEnumerable<Blog>> GetAllBlogPreviews(int pageIndex, int pageSize, string category, List<string> tags);

        public Task<IEnumerable<Modification>> GetAllBlogModifications(Guid id);

        public Task<IEnumerable<Rating>> GetAllBlogRatings(Guid id);

        public Task<IEnumerable<Comment>> GetAllBlogComments(Guid id)


    }
}

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

        public Task<IEnumerable<Rating>> GetAllBlogRatings(Guid blogId);

        public Task<IEnumerable<Comment>> GetAllBlogComments(Guid id, int pageIndex, int pageSize);

        public Task<Rating?> GetUserRatingforBlog(Guid blogId, Guid userId);

        public Task<IEnumerable<Category?>> GetAllBlogCategories(Guid blogId);
        public Task<int> GetBlogCategoryCountAsync(Guid blogId);

        public Task<int> GetBlogCountAsync();

        //keywords
        public Task<IEnumerable<Keyword?>> GetAllBlogKeywords(Guid blogId);
        public Task<int> GetBlogKeywordsCountAsync(Guid blogId);

        //images
        public Task<IEnumerable<BlogImage?>> GetAllBlogImages(Guid blogId);
        public Task<BlogImage?> GetFirstBlogImage(string blogGuid);


        public Task<bool> IsSubscriptionRequired(Guid blogId);


        public Task<IEnumerable<Blog>> GetAllBlogPreviewsByKeyword(Guid keywordId, int pageIndex, int pageSize);

        //Tags
        public Task<IEnumerable<Tag?>> GetAllBlogTags(Guid blogId);
        public Task<int> GetBlogTagsCountAsync(Guid blogId);



    }
}

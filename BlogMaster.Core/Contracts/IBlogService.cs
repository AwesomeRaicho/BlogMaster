using BlogMaster.Core.DTO;
using BlogMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Contracts
{
    public interface IBlogService
    {
        // Blog CRUD Operations
        public Task<Blog> GetBlogByIdAsync(Guid id);
        public Task<Blog> GetBlogBySlug(string slug);
        public Task<IEnumerable<Blog>> GetAllBlogsAsync();
        public Task<IEnumerable<Blog>> GetAllBlogsNextPage(int pageIndex);
        public Task CreateBlogAsync(Blog blog);
        public Task UpdateBlogAsync(Blog blog);
        public Task DeleteBlogAsync(Guid id);

        // Blog Publication Management
        public Task PublishBlogAsync(Guid id);
        public Task UnpublishBlogAsync(Guid id);

        // Blog Feature Management
        public Task FeatureBlogAsync(Guid id);
        public Task UnfeatureBlogAsync(Guid id);





        // Relationship Management
        public Task AddCategoryToBlogAsync(Guid blogId, Guid categoryId);
        public Task RemoveCategoryFromBlogAsync(Guid blogId, Guid categoryId);

        public Task AddKeywordToBlogAsync(Guid blogId, Guid keywordId);
        public Task RemoveKeywordFromBlogAsync(Guid blogId, Guid keywordId);

        public Task AddTagToBlogAsync(Guid blogId, Guid tagId);
        public Task RemoveTagFromBlogAsync(Guid blogId, Guid tagId);


        public Task AddCommentToBlogAsync(CommentPostPutDto commentPostPutDto);
        public Task DeleteCommentAsync(Guid blogId, Guid commentId);

        
        // BlogImage

        public Task AddImageToBlogAsync(BlogImagePostPutDto image);
        public Task RemoveImageFromBlogAsync(Guid blogId, Guid imageId);



        // Category Management
        public Task<Category> GetCategoryAsync(Guid categoryId);
        public Task<List<Category>> GetAllCategories();
        public Task CreateCategory(Category category);
        public Task DeleteCategory(Category category);
        public Task UpdateCategory(Category category);

        // Comment Management

        public Task<Comment> GetCommentAsync(Guid id);
        public Task<List<Comment>> GetAllCOmments();
        public Task CreateCommentAsync(Comment comment);
        public Task DeleteCommentAsync(Comment comment);
        public Task UpdateCommentAsync(Comment comment);

        // Keyword Management
        public Task<Keyword> GetKeywordAsync(Guid keywordId);
        public Task<List<Keyword>> GetAllKeywordsAsync();
        public Task CreateKeywordAsync(Keyword keyword);
        public Task DeleteKeywordAsync(Keyword keyword);
        public Task UpdateKeywordAsync(Keyword keyword);

        // Modifications Management
        public Task<Modification> GetModificationAsync(Guid id);
        public Task AddModificationToBlogAsync(ModificationPostPutDto modification);
        public Task<IEnumerable<Modification>> GetBlogModificationsAsync(Guid blogId);
        public Task DeleteBlogModificationsAsync(Modification modification);
        public Task UpdateBlogModigicationAsync(Guid blogId, Modification modification);

        // Rating Management

        public Task<Rating> GetRatingAsync(Guid blogId);
        public Task AddRatingToBlogAsync(RatingPostPutDto rating);
        public Task<List<Rating>> GetBlogRatingsAsync(Guid blogId);
        public Task DeleteBlogRatingsAsync(Guid ratingId);
        public Task UpdateBlogRatingsAsync(Rating rating);
        
        // Tag MAnagement
        public Task GetTagAsync(Guid tagId);
        public Task CreateTagAsync(Tag tag);
        public Task DeleteTagAsync(Tag tag);
        public Task UpdateTagAsync(Tag tag);
        public Task<List<Tag>> GetAllTagsAsync();



        // Other Utility Methods
        public Task<IEnumerable<Blog>> SearchBlogsAsync(string keyword);
        public Task<int> GetBlogViewCountAsync(Guid blogId);
        public Task<decimal> GetBlogAverageRatingAsync(Guid blogId);
        public Task<bool> IsSubscriptionRequiredAsync(Guid blogId);

        // Metadata or Information Methods
        public Task<IEnumerable<Category>> GetAllCategoriesAsync();
    }
}

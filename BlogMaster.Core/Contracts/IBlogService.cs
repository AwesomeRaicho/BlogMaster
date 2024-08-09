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
        public Task<BlogResponseDto> GetBlogByIdAsync(Guid id);
        public Task<BlogResponseDto> GetBlogBySlug(string slug);
        public Task<IEnumerable<BlogPreviewDto>> GetAllBlogPreviews(int pageIndex, int pageSize, string category, List<string> tags);
        public Task CreateBlogAsync(BlogPostPutDto blog);
        public Task UpdateBlogAsync(BlogPostPutDto blog);
        public Task DeleteBlogAsync(Guid id);
        public Task<IEnumerable<CommentResponseDto>> GetAllBlogComments(Guid blogId, int pageIndex, int pageSize);

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

        
        // BlogImage

        public Task AddImageToBlogAsync(BlogImagePostPutDto image);
        public Task RemoveImageFromBlogAsync(Guid blogId, Guid imageId);



        // Category Management
        public Task<Category> GetCategoryAsync(Guid categoryId);
        public Task<IEnumerable<CategoryResponseDto>> GetAllCategories();
        public Task CreateCategoryAsync(CategoryPostPutDto category);
        public Task DeleteCategoryAsync(Guid id);
        public Task UpdateCategory(Category category);

        // Comment Management

        public Task<Comment> GetCommentAsync(Guid id);
        public Task<IEnumerable<CommentResponseDto>> GetAllCommentsAsync();
        public Task CreateCommentAsync(CommentPostPutDto comment);
        public Task DeleteCommentAsync(Guid id);
        public Task UpdateCommentAsync(Comment comment);

        // Keyword ManagementCreateTagAsync
        public Task<Keyword> GetKeywordAsync(Guid keywordId);
        public Task<IEnumerable<KeywordResponseDto>> GetAllKeywordsAsync();
        public Task CreateKeywordAsync(KeywordPostPut keyword);
        public Task DeleteKeywordAsync(Guid id);
        public Task UpdateKeywordAsync(Keyword keyword);

        // Modifications Management
        public Task<Modification> GetModificationAsync(Guid id);
        public Task AddModificationToBlogAsync(ModificationPostPutDto modification);
        public Task<IEnumerable<Modification>> GetBlogModificationsAsync(Guid blogId);
        public Task DeleteBlogModificationsAsync(Guid id);
        public Task UpdateBlogModigicationAsync(Guid blogId, Modification modification);

        // Rating Management

        public Task<Rating> GetRatingAsync(Guid blogId);
        public Task AddRatingToBlogAsync(RatingPostPutDto rating);
        public Task DeleteRatingAsync(Guid id);
        public Task UpdateRatingAsync(Rating id);
        
        // Tag MAnagement
        public Task GetTagAsync(Guid tagId);
        public Task CreateTagAsync(TagPostPutDto tag);
        public Task DeleteTagAsync(Guid id);
        public Task UpdateTagAsync(Tag tag);
        public Task<IEnumerable<TagResponseDto>> GetAllTagsAsync();



        // Other Utility Methods
        public Task<IEnumerable<Blog>> SearchBlogsAsync(string keyword);
        public Task<int> GetBlogViewCountAsync(Guid blogId);
        public Task<bool> IsSubscriptionRequiredAsync(Guid blogId);


    }
}

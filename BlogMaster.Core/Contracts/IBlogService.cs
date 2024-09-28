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
        public Task<List<AdminBlogListDto>> GetAllAdminBlogPreviews(int pageIndex, string category, List<string> tags);

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
        public Task RemoveImageAsync(Guid imageId);



        // Category Management
        public Task<CategoryResponseDto> GetCategoryAsync(Guid categoryId);
        public Task<IEnumerable<CategoryResponseDto>> GetAllCategories();
        public Task CreateCategoryAsync(CategoryPostPutDto category);
        public Task DeleteCategoryAsync(Guid id);
        public Task UpdateCategory(CategoryPostPutDto categoryPostPutDto);


        // Comment Management
        public Task<CommentResponseDto> GetCommentAsync(Guid id);
        public Task<IEnumerable<CommentResponseDto>> GetAllCommentsAsync();
        public Task CreateCommentAsync(CommentPostPutDto commentPostPutDto);
        public Task DeleteCommentAsync(Guid id);
        public Task UpdateCommentAsync(CommentPostPutDto commentPostPutDto);


        // Keyword ManagementCreateTagAsync
        public Task<KeywordResponseDto> GetKeywordAsync(Guid keywordId);
        public Task<IEnumerable<KeywordResponseDto>> GetAllKeywordsAsync();
        public Task CreateKeywordAsync(KeywordPostPut keyword);
        public Task DeleteKeywordAsync(Guid id);
        public Task UpdateKeywordAsync(KeywordPostPut keywordPostPut);


        // Modifications Management
        public Task<ModificationResponseDto> GetModificationAsync(Guid id);
        public Task AddModificationToBlogAsync(ModificationPostPutDto modification);
        public Task<IEnumerable<ModificationResponseDto>> GetBlogModificationsAsync(Guid blogId);
        public Task DeleteBlogModificationsAsync(Guid id);
        public Task UpdateBlogModigicationAsync(ModificationPostPutDto modification);


        // Rating Management

        public Task<RatingResponseDto> GetRatingAsync(Guid blogId);
        public Task AddRatingToBlogAsync(RatingPostPutDto rating);
        public Task DeleteRatingAsync(Guid id);
        public Task UpdateRatingAsync(RatingPostPutDto ratingPostPutDto);

        
        // Tag Management
        public Task<TagResponseDto> GetTagAsync(Guid tagId);
        public Task CreateTagAsync(TagPostPutDto tag);
        public Task DeleteTagAsync(Guid id);
        public Task UpdateTagAsync(TagPostPutDto tagPostPutDto);
        public Task<IEnumerable<TagResponseDto>> GetAllTagsAsync();



        // Other Utility Methods
        public Task<IEnumerable<BlogPreviewDto>> SearchBlogsWithKeywordAsync(Guid keywordId, int pageIndex, int pageSize);
        public Task<bool> IsSubscriptionRequiredAsync(Guid blogId);


    }
}

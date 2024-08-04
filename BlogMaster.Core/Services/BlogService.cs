using BlogMaster.Core.Contracts;
using BlogMaster.Core.DTO;
using BlogMaster.Core.Models;
using BlogMaster.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Services
{
    public class BlogService : IBlogService
    {

        private readonly IRepository<Blog> _blogRepository;
        private readonly IRepository<BlogImage> _blogImageRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<Keyword> _keywordRepository;
        private readonly IRepository<Modification> _modificationRepository;
        private readonly IRepository<Rating> _ratingRepository;
        private readonly IRepository<Tag> _tagRepository;



        private readonly IRepository<Blog_Category> _blogCategoryRepository;
        private readonly IRepository<Blog_Keyword> _blogKeywordRepository;
        private readonly IRepository<Blog_Tag> _blogTagRepository;


        


        public BlogService(IRepository<Blog> blogRepository, IRepository<Category> categoryRepository, IRepository<Comment> commentRepository, IRepository<Keyword> keywordRepository, IRepository<Modification> modificationRepository, IRepository<Rating> ratingRepository, IRepository<Tag> tagRepository, IRepository<Blog_Category> blogCategoryRepository, IRepository<Blog_Keyword> blogKeywordRepository, IRepository<Blog_Tag> blogTagRepository)
        {
            _blogRepository = blogRepository;
            _categoryRepository = categoryRepository;
            _commentRepository = commentRepository;
            _keywordRepository = keywordRepository;
            _modificationRepository = modificationRepository;
            _ratingRepository = ratingRepository;
            _tagRepository = tagRepository;
            _blogCategoryRepository = blogCategoryRepository;
            _blogKeywordRepository = blogKeywordRepository;
            _blogTagRepository = blogTagRepository;
        }

        public async Task AddCategoryToBlogAsync(Guid blogId, Guid categoryId)
        {
            Blog_Category entity = new Blog_Category()
            {
                BlogId = blogId,
                CategoryId = categoryId
            };

            await _blogCategoryRepository.Create(entity);

        }

        public async Task AddCommentToBlogAsync(CommentPostPutDto commentPostPutDto)
        {


            if(commentPostPutDto == null || string.IsNullOrEmpty(commentPostPutDto.Message))
            {
                throw new ArgumentNullException("Comment cant be null");
            }


            Comment comment = new Comment()
            {
                BlogId = commentPostPutDto.BlogId,
                Message = commentPostPutDto.Message,
                UserId = commentPostPutDto.UserId,
                CommentId = Guid.NewGuid(),
            };

            await _commentRepository.Create(comment);
        }

        public async Task AddImageToBlogAsync(BlogImagePostPutDto image)
        {
            if(image == null || image.ImageData == null)
            {
                throw new ArgumentNullException("Image cannot be null");
            }
            if( string.IsNullOrEmpty(image.ImageName))
            {
                throw new ArgumentNullException("Image name cannot be null");

            }

            BlogImage imageEntity = new BlogImage()
            {
                ImageId = Guid.NewGuid(),
                BlogId = image.BlogId,
                ImageData = image.ImageData,
                MimeType = image.MimeType,
                ImageName = image.ImageName,
                Url = image.Url
            };

            await _blogImageRepository.Create(imageEntity);

        }

        public async Task AddKeywordToBlogAsync(Guid blogId, Guid keywordId)
        {
            Blog_Keyword entity = new Blog_Keyword()
            {
                BlogId = blogId,
                KeywordId = keywordId
            };

            await _blogKeywordRepository.Create(entity);
        }

        public async Task AddModificationToBlogAsync(ModificationPostPutDto modification)
        {
            if(modification == null)
            {
                throw new ArgumentNullException("Modification cannot be null.");
            };
            if(string.IsNullOrEmpty(modification.Description))
            {
                throw new ArgumentNullException("Modification Description must be added.");

            };

            Modification entity = new Modification()
            {
                ModificationId = Guid.NewGuid(),
                BlogId= modification.BlogId,
                Description = modification.Description,
                ModificationDate = DateTime.Now,
                UserId = modification.UserId,
            };

            await _modificationRepository.Create(entity);

        }


        public async Task AddRatingToBlogAsync(RatingPostPutDto rating)
        {
            if (rating == null || rating.RatingScore == null)
            {
                throw new ArgumentNullException("Rating cannot be null");
            }

            Rating entity = new Rating()
            {
                RatingId = Guid.NewGuid(),
                BlogId = rating.BlogId,
                RatingScore = rating.RatingScore,
                UserId = rating.UserId,
            };

            await _ratingRepository.Create(entity);
        }

        public async Task AddTagToBlogAsync(Guid blogId, Guid tagId)
        {
            Blog_Tag entity = new Blog_Tag()
            {
                BlogId = blogId,
                TagId = tagId
            };

            await _blogTagRepository.Create(entity);
        }

        public async Task CreateBlogAsync(BlogPostPutDto blog)
        {
            if (blog == null)
            {
                throw new ArgumentNullException("Blog Cannot be null");
            }

            if(string.IsNullOrEmpty(blog.ArticleEn) && string.IsNullOrEmpty(blog.ArticleEs))
            {
                throw new ArgumentNullException("Article must be added in at least one lenguage");
            }
            if (string.IsNullOrEmpty(blog.TitleEn) && string.IsNullOrEmpty(blog.TitleEs))
            {
                throw new ArgumentNullException("Title must be added in at least one lenguage");
            }
            if (string.IsNullOrEmpty(blog.DescriptionEn) && string.IsNullOrEmpty(blog.DescriptionEs))
            {
                throw new ArgumentNullException("Description must be added in at least one lenguage");
            }

            if(string.IsNullOrEmpty(blog.Author))
            {
                throw new ArgumentNullException("Author cannot be null");
            }

            Blog entity = new Blog()
            {
                BlogId = Guid.NewGuid(),
                ArticleEn = blog.ArticleEn,
                ArticleEs = blog.ArticleEs,
                TitleEn = blog.TitleEn,
                TitleEs = blog.TitleEs,
                DescriptionEs = blog.DescriptionEs,
                DescriptionEn = blog.DescriptionEn,
                SlugEn = !string.IsNullOrEmpty(blog.TitleEn) ? SlugGenerator.GenerateSlug(blog.TitleEn) : null,
                SlugEs = !string.IsNullOrEmpty(blog.TitleEs) ? SlugGenerator.GenerateSlug(blog.TitleEs) : null,
                Author = blog.Author,
                IsSubscriptionRequired = blog.IsSubscriptionRequired,
                
                
                
                IsFeatured = true,
                IsPublished = false,

                CreatedDate = DateTime.UtcNow,
                DatePublished = null,

                ViewCount = 0,
                AverageRating = null,
                RatingCount = 0,


            };

            await _blogRepository.Create(entity);

        }

        public async Task CreateCategory(CategoryPostPutDto category)
        {
            if(category == null) { throw new ArgumentNullException("Categoty submitted cannot be mull"); }

            if(string.IsNullOrEmpty(category.CatergoryNameEn) && string.IsNullOrEmpty(category.CatergoryNameEs))
            {
                throw new ArgumentNullException("Category name must be provided at least in one lenguage.");
            }

            Category entity = new Category()
            {
                CategoryId = Guid.NewGuid(),
                CatergoryNameEn = category.CatergoryNameEn,
                CatergoryNameEs = category.CatergoryNameEs,
            };

            await _categoryRepository.Create(entity);

        }

        public async Task CreateCommentAsync(CommentPostPutDto comment)
        {
            if(comment == null || string.IsNullOrEmpty(comment.Message))
            {
                throw new ArgumentNullException("Comment cannot be null");
            }

            Comment entity = new Comment()
            {
                CommentId = Guid.NewGuid(),
                BlogId = comment.BlogId,
                UserId = comment.UserId,
                Message = comment.Message,
            };

            await _commentRepository.Create(entity);
        }

        public async Task CreateKeywordAsync(KeywordPostPut keyword)
        {
            if(keyword == null)
            {
                throw new ArgumentNullException("Keyword cannot be null");
            }

            if(string.IsNullOrEmpty(keyword.KeywordNameEn) && string.IsNullOrEmpty(keyword.KeywordNameEs))
            {
                throw new ArgumentNullException("Keyword must be provided in at least one lenguage.");
            }

            Keyword entity = new Keyword()
            {
                KeywordId = Guid.NewGuid(),
                KeywordNameEn = keyword.KeywordNameEn,
                KeywordNameEs = keyword.KeywordNameEs,
            };

            await _keywordRepository.Create(entity);

        }

        public Task CreateTagAsync(Tag tag)
        {
            throw new NotImplementedException();
        }

        public Task DeleteBlogAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteBlogModificationsAsync(Modification modification)
        {
            throw new NotImplementedException();
        }

        public Task DeleteBlogRatingsAsync(Guid ratingId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCategory(Category category)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCommentAsync(Guid blogId, Guid commentId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCommentAsync(Comment comment)
        {
            throw new NotImplementedException();
        }

        public Task DeleteKeywordAsync(Keyword keyword)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRatingAsync(Guid blogId, Guid ratingId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTagAsync(Tag tag)
        {
            throw new NotImplementedException();
        }

        public Task FeatureBlogAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Blog>> GetAllBlogsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Blog>> GetAllBlogsNextPage(int pageIndex)
        {
            throw new NotImplementedException();
        }

        public Task<List<Category>> GetAllCategories()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Comment>> GetAllCOmments()
        {
            throw new NotImplementedException();
        }

        public Task<List<Keyword>> GetAllKeywordsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Tag>> GetAllTagsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetBlogAverageRatingAsync(Guid blogId)
        {
            throw new NotImplementedException();
        }

        public Task<Blog> GetBlogByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Blog> GetBlogBySlug(string slug)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Modification>> GetBlogModificationsAsync(Guid blogId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Rating>> GetBlogRatingsAsync(Guid blogId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetBlogViewCountAsync(Guid blogId)
        {
            throw new NotImplementedException();
        }

        public Task<Category> GetCategoryAsync(Guid categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<Comment> GetCommentAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Keyword> GetKeywordAsync(Guid keywordId)
        {
            throw new NotImplementedException();
        }

        public Task<Modification> GetModificationAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Rating> GetRatingAsync(Guid blogId)
        {
            throw new NotImplementedException();
        }

        public Task GetTagAsync(Guid tagId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsSubscriptionRequiredAsync(Guid blogId)
        {
            throw new NotImplementedException();
        }

        public Task PublishBlogAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task RemoveCategoryFromBlogAsync(Guid blogId, Guid categoryId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveImageFromBlogAsync(Guid blogId, Guid imageId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveKeywordFromBlogAsync(Guid blogId, Guid keywordId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveTagFromBlogAsync(Guid blogId, Guid tagId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Blog>> SearchBlogsAsync(string keyword)
        {
            throw new NotImplementedException();
        }

        public Task UnfeatureBlogAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UnpublishBlogAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateBlogAsync(Blog blog)
        {
            throw new NotImplementedException();
        }

        public Task UpdateBlogModigicationAsync(Guid blogId, Modification modification)
        {
            throw new NotImplementedException();
        }

        public Task UpdateBlogRatingsAsync(Rating rating)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCategory(Category category)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCommentAsync(Comment comment)
        {
            throw new NotImplementedException();
        }

        public Task UpdateKeywordAsync(Keyword keyword)
        {
            throw new NotImplementedException();
        }

        public Task UpdateRatingAsync(Guid blogId, Rating rating)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTagAsync(Tag tag)
        {
            throw new NotImplementedException();
        }
    }
}

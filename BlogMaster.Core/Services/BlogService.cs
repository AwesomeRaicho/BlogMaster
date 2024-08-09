using BlogMaster.Core.Contracts;
using BlogMaster.Core.DTO;
using BlogMaster.Core.Models;
using BlogMaster.Core.Utilities;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection.Metadata;
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
        private readonly IBlogRepository _blogUniqueRepository;



        private readonly IRepository<Blog_Category> _blogCategoryRepository;
        private readonly IRepository<Blog_Keyword> _blogKeywordRepository;
        private readonly IRepository<Blog_Tag> _blogTagRepository;


        // add a way to calculate the avergae rating of a blog
        


        public BlogService(IRepository<Blog> blogRepository, IRepository<Category> categoryRepository, IRepository<Comment> commentRepository, IRepository<Keyword> keywordRepository, IRepository<Modification> modificationRepository, IRepository<Rating> ratingRepository, IRepository<Tag> tagRepository, IRepository<Blog_Category> blogCategoryRepository, IRepository<Blog_Keyword> blogKeywordRepository, IRepository<Blog_Tag> blogTagRepository, IBlogRepository blogUniqueRepository)
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
            _blogUniqueRepository = blogUniqueRepository;
        }



        //PRIVATE METHODS

        /// <summary>
        /// Provides a way for methods to create a blog response dto with all other related tables included
        /// </summary>
        /// <param name="blog">Blog entity from DB </param>
        /// <returns>BlogResponseDto</returns>
        /// <exception cref="NotImplementedException"></exception>
        private async Task<BlogResponseDto> CreateBlogResponseDto(Blog blog)
        {
            //create BlogResponseDto ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            BlogResponseDto blogResponseDto = new BlogResponseDto()
            {
                BlogId = blog.BlogId,
                ArticleEn = blog.ArticleEn,
                ArticleEs = blog.ArticleEs,
                TitleEn = blog.TitleEn,
                TitleEs = blog.TitleEs,
                DescriptionEn = blog.DescriptionEn,
                DescriptionEs = blog.DescriptionEs,
                SlugEn = blog.SlugEn,
                SlugEs = blog.SlugEs,
                Author = blog.Author,
                DatePublished = blog.DatePublished,
                ViewCount = blog.ViewCount,
                AverageRating = blog.AverageRating,
                RatingCount = blog.RatingCount,
            };


            //GET COMMENTS WITH USER RATING FOR THE BLOG
            //get the first visible comments
            List<Comment> comments = (await _blogUniqueRepository.GetAllBlogComments(blog.BlogId, 1, 20)).ToList();
            //if comments != null get rating from the users if any.
            if (comments.Count > 0 && comments != null)
            {
                //create cache for user ratings
                Dictionary<string, Rating> ratingCache = new Dictionary<string, Rating>();

                //create list that will go in BlogResponseDTO
                List<CommentRatingResponseDto> commentRatings = new List<CommentRatingResponseDto>();


                foreach (Comment comment in comments)
                {

                    string userIdString = comment.UserId.ToString();

                    // Check cache for rating
                    if (!ratingCache.TryGetValue(userIdString, out Rating? rating))
                    {
                        // If not found in cache, fetch from repository
                        rating = await _blogUniqueRepository.GetUserRatingforBlog(comment.BlogId, comment.UserId);

                        // Add fetched rating to cache
                        if (rating != null)
                        {
                            ratingCache[userIdString] = rating;
                        }
                    }

                    CommentRatingResponseDto commentrating = new CommentRatingResponseDto()
                    {
                        CommentId = comment.CommentId,
                        Message = comment.Message,
                        RatingScore = rating != null ? rating.RatingScore : 0,
                        UserId = comment.UserId,
                        UserName = comment.UserName

                    };


                    commentRatings.Add(commentrating);

                }
                // add 1st page of comments + rating to the BlogResponseDto ~~~~~~~~~~~~~~~~~~~~~~~~~~
                blogResponseDto.CommentsRatings = commentRatings;
            }



            //GET CATEGORIES
            List<CategoryResponseDto> categories = new List<CategoryResponseDto>();

            IEnumerable<Category?> allBlogCategories = await _blogUniqueRepository.GetAllBlogCategories(blogResponseDto.BlogId);

            foreach (Category? category in allBlogCategories)
            {
                if (category == null) continue;
                CategoryResponseDto categoryResponse = new CategoryResponseDto()
                {
                    CategoryId = category.CategoryId,
                    CatergoryNameEn = category.CatergoryNameEn,
                    CatergoryNameEs = category.CatergoryNameEs,
                };

                categories.Add(categoryResponse);

            }

            //add categories to the BlogResponseDto ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            blogResponseDto.Categories = categories;



            //GET KEYWORDS
            List<KeywordResponseDto> keywordResponseDtos = new List<KeywordResponseDto>();

            IEnumerable<Keyword?> keywords = await _blogUniqueRepository.GetAllBlogKeywords(blogResponseDto.BlogId);

            foreach (Keyword? keyword in keywords)
            {
                if (keyword == null) continue;

                KeywordResponseDto keywordResponse = new KeywordResponseDto()
                {
                    KeywordId = keyword.KeywordId,
                    KeywordNameEn = keyword.KeywordNameEn,
                    KeywordNameEs = keyword.KeywordNameEs,
                };

                keywordResponseDtos.Add(keywordResponse);
            }

            //add Keywords to the BlogResponseDto ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            blogResponseDto.Keywords = keywordResponseDtos;


            //GET BLOGIMAGES
            List<BlogImagesResponseDto> imageresponses = new List<BlogImagesResponseDto>();

            IEnumerable<BlogImage?> images = await _blogUniqueRepository.GetAllBlogImages(blogResponseDto.BlogId);

            foreach (BlogImage? image in images)
            {
                if (image == null) continue;

                BlogImagesResponseDto imageResponse = new BlogImagesResponseDto()
                {
                    BlogImageId = image.ImageId,
                    ImageData = image.ImageData,
                    ImageName = image.ImageName,
                    MimeType = image.MimeType,
                    Url = image.Url,
                };

                imageresponses.Add(imageResponse);
            }

            //add BlogImages to the BlogResponseDto ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            blogResponseDto.BlogImages = imageresponses;

            return blogResponseDto;

        }









        /////////////////////////////////////////////////////////////////////////////////
        //INTERFACE METHODS

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

        public async Task CreateCategoryAsync(CategoryPostPutDto category)
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

        public async Task CreateTagAsync(TagPostPutDto tag)
        {
            if(tag == null)
            {
                throw new ArgumentNullException("tag cannot be null");
            }

            if(string.IsNullOrEmpty(tag.TagNameEn) && string.IsNullOrEmpty(tag.TagNameEs))
            {
                throw new ArgumentNullException("tag cannot be null");
            }

            Tag entity = new Tag()
            {
                TagId = Guid.NewGuid(),
                TagNameEn = tag.TagNameEn,
                TagNameEs = tag.TagNameEs,
            };

            await _tagRepository.Create(entity);
        }

        public async Task DeleteBlogAsync(Guid id)
        {
            await _blogRepository.Delete(id);
        }

        public async Task DeleteBlogModificationsAsync(Guid id)
        {
            await _modificationRepository.Delete(id);
        }

        public async Task DeleteBlogRatingsAsync(Guid id)
        {
            await _ratingRepository.Delete(id);
        }

        public async Task DeleteCategoryAsync(Guid id)
        {
            await _categoryRepository.Delete(id);
        }

        public async Task DeleteCommentAsync(Guid id)
        {
            await _commentRepository.Delete(id);
        }


        public async Task DeleteKeywordAsync(Guid id)
        {
            await _keywordRepository.Delete(id);
        }

        public async Task DeleteRatingAsync(Guid id)
        {
            await _ratingRepository.Delete(id);
        }

        public async Task DeleteTagAsync(Guid id)
        {
            await _tagRepository.Delete(id);
        }

        public async Task FeatureBlogAsync(Guid id)
        {
            Blog? blog = await _blogRepository.Get(id);
            if(blog == null)
            {
                throw new Exception("Blog Id does not exist");
            }
            blog.IsFeatured = true;
            await _blogRepository.Update(blog);
        }

        public async Task<IEnumerable<BlogPreviewDto>> GetAllBlogPreviews(int pageIndex, int pageSize, string category, List<string> tags)
        {
            IEnumerable<Blog> blogs = await _blogUniqueRepository.GetAllBlogPreviews(pageIndex, pageSize, category, tags);

            List<BlogPreviewDto> result = new List<BlogPreviewDto>();

            foreach(Blog preview in blogs)
            {
                BlogPreviewDto blogPreviewDto = new BlogPreviewDto()
                {
                    BlogId = preview.BlogId,
                    Author = preview.Author,
                    AverageRating = preview.AverageRating,
                    DatePublished = preview.DatePublished,
                    DescriptionEn = preview.DescriptionEn,
                    DescriptionEs = preview.DescriptionEs,
                    SlugEn = preview.SlugEn,
                    SlugEs = preview.SlugEs,
                    TitleEn = preview.TitleEn,
                    TitleEs = preview.TitleEs,
                };

                result.Add(blogPreviewDto);
            }

            return result;

        }


        public async Task<IEnumerable<CategoryResponseDto>> GetAllCategories()
        {
            var categories =  await _categoryRepository.GetAll(1, 100);

            List<CategoryResponseDto> categoryResponseDtos = new List<CategoryResponseDto>();

            foreach (var category in categories)
            {
                CategoryResponseDto categoryResponseDto = new CategoryResponseDto()
                {
                    CategoryId = category.CategoryId,
                    CatergoryNameEn = category.CatergoryNameEn,
                    CatergoryNameEs = category.CatergoryNameEs,
                };
            }

            return categoryResponseDtos;
        }



        public async Task<IEnumerable<CommentResponseDto>> GetAllCommentsAsync()
        {
            IEnumerable<Comment> comments = await _commentRepository.GetAll(1, 100);
            List<CommentResponseDto> commentResponseDtos = new List<CommentResponseDto>();

            foreach (Comment comment in comments)
            {
                CommentResponseDto commentResponseDto = new CommentResponseDto()
                {
                    BlogId = comment.BlogId,
                    CommentId = comment.CommentId,
                    Message = comment.Message,
                    UserId = comment.UserId,
                    UserName = comment.UserName
                };

                commentResponseDtos.Add(commentResponseDto);
            }

            return commentResponseDtos;
        }

        public async Task<IEnumerable<KeywordResponseDto>> GetAllKeywordsAsync()
        {

            var Keywords =  await _keywordRepository.GetAll(1, 100);
            List<KeywordResponseDto> keywords = new List<KeywordResponseDto>();

            foreach(Keyword keyword in Keywords)
            {
                KeywordResponseDto keywordResponseDto = new KeywordResponseDto()
                {
                    KeywordId = keyword.KeywordId,
                    KeywordNameEn = keyword.KeywordNameEn,
                    KeywordNameEs = keyword.KeywordNameEs,
                };

                keywords.Add(keywordResponseDto);
            }
            return keywords;

        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            return await _tagRepository.GetAll(1, 100);

        }

        public async Task<IEnumerable<CommentResponseDto>> GetAllBlogComments(Guid blogId, int pageIndex, int pageSize)
        {
            IEnumerable<Comment> comments =  await _blogUniqueRepository.GetAllBlogComments(blogId, pageIndex, pageSize);
            List<CommentResponseDto> commentResponseDtos = new List<CommentResponseDto>();

            foreach(Comment comment in comments)
            {
                CommentResponseDto commentResponseDto = new CommentResponseDto()
                {
                    BlogId = comment.BlogId,
                    CommentId = comment.CommentId,
                    Message = comment.Message,
                    UserId = comment.UserId,
                    UserName = comment.UserName
                };

                commentResponseDtos.Add(commentResponseDto);
            }

            return commentResponseDtos;
        }






        public async Task<BlogResponseDto> GetBlogByIdAsync(Guid blogId)
        {
            Blog? blog = await _blogRepository.Get(blogId);

            if (blog == null)
            {
                throw new Exception("Blog does not exist");
            }

            BlogResponseDto response = await this.CreateBlogResponseDto(blog);


            return response;
        }

        public async Task<BlogResponseDto> GetBlogBySlug(string slug)
        {
            Blog? blog = await _blogUniqueRepository.GetBlogBySlug(slug);

            if (blog == null)
            {
                throw new Exception("Blog does not exist");
            }

            blog.ViewCount += 1;
            await _blogRepository.Update(blog);

            BlogResponseDto response = await this.CreateBlogResponseDto(blog);


            return response;
        }

        public async Task<IEnumerable<Modification>> GetBlogModificationsAsync(Guid blogId)
        {
            return await _blogUniqueRepository.GetAllBlogModifications(blogId);
        }

        public async Task<IEnumerable<Rating>> GetBlogRatingsAsync(Guid blogId)
        {
            return await _blogUniqueRepository.GetAllBlogRatings(blogId);
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

        public async Task UnfeatureBlogAsync(Guid id)
        {
            Blog? blog = await _blogRepository.Get(id);
            if (blog == null)
            {
                throw new Exception("Blog Id does not exist");
            }
            blog.IsFeatured = false;
            await _blogRepository.Update(blog);
        }

        public Task UnpublishBlogAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateBlogAsync(BlogPostPutDto blog)
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

        public Task UpdateRatingAsync(Rating rating)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTagAsync(Tag tag)
        {
            throw new NotImplementedException();
        }
    }
}

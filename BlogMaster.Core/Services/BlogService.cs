using BlogMaster.Core.Contracts;
using BlogMaster.Core.DTO;
using BlogMaster.Core.Models;
using BlogMaster.Core.Models.Identity;
using BlogMaster.Core.Utilities;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace BlogMaster.Core.Services
{
    public class BlogService : IBlogService
    {
        private readonly IRepository<ApplicationUser> _applicationUserRepository;
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
        


        public BlogService(IRepository<Blog> blogRepository, IRepository<Category> categoryRepository, IRepository<Comment> commentRepository, IRepository<Keyword> keywordRepository, IRepository<Modification> modificationRepository, IRepository<Rating> ratingRepository, IRepository<Tag> tagRepository, IRepository<Blog_Category> blogCategoryRepository, IRepository<Blog_Keyword> blogKeywordRepository, IRepository<Blog_Tag> blogTagRepository, IBlogRepository blogUniqueRepository, IRepository<BlogImage> blogImageRepository, IRepository<ApplicationUser> applicationUser)
        {
            _applicationUserRepository = applicationUser;
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
            _blogImageRepository = blogImageRepository;
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
                IsFeatured = blog.IsFeatured,
                IsPublished = blog.IsPublished,
                IsSubscriptionRequired = blog.IsSubscriptionRequired,
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
                    CategoryNameEn = category.CatergoryNameEn,
                    CategoryNameEs = category.CatergoryNameEs,
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

            // GET TAGS
            List<TagResponseDto> tagResponseDtos = new List<TagResponseDto>();

            // Fetch all tags associated with the blog
            IEnumerable<Tag?> tags = await _blogUniqueRepository.GetAllBlogTags(blogResponseDto.BlogId);

            foreach (Tag? tag in tags)
            {
                if (tag == null) continue;

                TagResponseDto tagResponse = new TagResponseDto()
                {
                    TagId = tag.TagId,
                    TagNameEn = tag.TagNameEn,
                    TagNameEs = tag.TagNameEs,
                };

                tagResponseDtos.Add(tagResponse);
            }

            // Add tags to the BlogResponseDto
            blogResponseDto.Tags = tagResponseDtos;



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
                Created = DateTime.UtcNow,
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

            Rating? entitycheck = await _ratingRepository.Find(e => e.UserId == rating.UserId && e.BlogId == rating.BlogId);

            if(entitycheck != null)
            {
                return;
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
                UserId = blog.UserId,
                ArticleEn = blog.ArticleEn,
                ArticleEs = blog.ArticleEs,
                TitleEn = blog.TitleEn,
                TitleEs = blog.TitleEs,
                DescriptionEs = blog.DescriptionEs,
                DescriptionEn = blog.DescriptionEn,
                SlugEn = !string.IsNullOrEmpty(blog.TitleEn) ? SlugGenerator.GenerateSlug(blog.TitleEn) : null,
                SlugEs = !string.IsNullOrEmpty(blog.TitleEs) ? SlugGenerator.GenerateSlug(blog.TitleEs) : null,
                Author = blog.Author,
                IsSubscriptionRequired = blog.IsSubscriptionRequired == "true" ? true : false,
                
                
                
                IsFeatured = blog.IsFeatured == "true" ? true : false,
                IsPublished = blog.IsPublished == "true" ? true : false,

                CreatedDate = DateTime.UtcNow,
                DatePublished = blog.IsPublished == "true" ? DateTime.UtcNow : null ,

                ViewCount = 0,
                AverageRating = null,
                RatingCount = 0,


            };

            try
            {
                await _blogRepository.Create(entity);

            }
            catch(Exception ex)
            {
                throw new Exception($"BlogService:Could not createblog >> {ex.Message}");
            }

            // add list of categories
            if(blog.CategoriesIds != null)
            {
                foreach(string categoryid in blog.CategoriesIds)
                {
                    await AddCategoryToBlogAsync(entity.BlogId, Guid.Parse(categoryid));
                }
            }
            // add list of tags
            if (blog.TagsIds != null)
            {
                foreach (string TagsId in blog.TagsIds)
                {
                    await AddTagToBlogAsync(entity.BlogId, Guid.Parse(TagsId));
                }
            }


            // add list of keywords
            if (blog.KeywordsIds != null)
            {
                foreach (string keywordid in blog.KeywordsIds)
                {
                    await AddKeywordToBlogAsync(entity.BlogId, Guid.Parse(keywordid));
                }
            }



        }

        public async Task CreateCategoryAsync(CategoryPostPutDto category)
        {
            if(category == null) { throw new ArgumentNullException("Categoty submitted cannot be mull"); }

            if(string.IsNullOrEmpty(category.CategoryNameEn) && string.IsNullOrEmpty(category.CategoryNameEs))
            {
                throw new ArgumentNullException("Category name must be provided at least in one lenguage.");
            }

            Category entity = new Category()
            {
                CategoryId = Guid.NewGuid(),
                CatergoryNameEn = category.CategoryNameEn,
                CatergoryNameEs = category.CategoryNameEs,
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

        public async Task CreateKeywordAsync(KeywordPostPutDto keyword)
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

            var entity = await _blogRepository.Get(id);

            if(entity == null)
            {
                throw new Exception("Blog does not exist"); 
            }

            await _blogRepository.Delete(entity);
        }

        public async Task DeleteBlogModificationsAsync(Guid id)
        {
            var entity = await _modificationRepository.Get(id);
            if(entity == null)
            {
                throw new Exception("Modification does not exist");
            }

            await _modificationRepository.Delete(entity);
        }


        public async Task DeleteCategoryAsync(Guid id)
        {
            var entity = await _categoryRepository.Get(id);
            if(entity == null)
            {
                throw new Exception("Category does not exist");
            }

            await _categoryRepository.Delete(entity);
        }

        public async Task DeleteCommentAsync(Guid id)
        {
            var entity = await _commentRepository.Get(id);
            if (entity == null)
            {
                throw new Exception("Comment does not exist");
            }

            await _commentRepository.Delete(entity);
        }


        public async Task DeleteKeywordAsync(Guid id)
        {
            var entity = await _keywordRepository.Get(id);
            if (entity == null)
            {
                throw new Exception("Keyword does not exist");
            }
            await _keywordRepository.Delete(entity);
        }

        public async Task DeleteRatingAsync(Guid id)
        {
            var entity = await _ratingRepository.Get(id);
            if (entity == null)
            {
                throw new Exception("Keyword does not exist");
            }

            await _ratingRepository.Delete(entity);
        }

        public async Task DeleteTagAsync(Guid id)
        {
            var entity = await _tagRepository.Get(id);
            if (entity == null)
            {
                throw new Exception("Tag does not exist");
            }

            await _tagRepository.Delete(entity);
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

        public async Task<IEnumerable<PublicBlogListDto>> GetAllBlogPreviews(int pageIndex, int pageSize, string category, List<string> tags)
        {
            IEnumerable<Blog> blogs = await _blogUniqueRepository.GetAllBlogPreviews(pageIndex, pageSize, category, tags);

            List<PublicBlogListDto> result = new List<PublicBlogListDto>();

            foreach(Blog preview in blogs)
            {
                PublicBlogListDto blogPreviewDto = new PublicBlogListDto()
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
                    CategoryNameEn = category.CatergoryNameEn,
                    CategoryNameEs = category.CatergoryNameEs,
                };
                categoryResponseDtos.Add(categoryResponseDto);
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
                    UserName = comment.UserName,
                    Created = comment.Created,  
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

        public async Task<IEnumerable<TagResponseDto>> GetAllTagsAsync()
        {
            var Tags = await _tagRepository.GetAll(1, 100);

            List<TagResponseDto> tagResponseDtos = new List<TagResponseDto>();

            foreach (Tag tag in Tags)
            {
                TagResponseDto tagResponseDto = new TagResponseDto()
                {
                    TagId = tag.TagId,
                    TagNameEn = tag.TagNameEn,
                    TagNameEs = tag.TagNameEs,
                    
                };

                tagResponseDtos.Add(tagResponseDto);
            }

            return tagResponseDtos;
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

            BlogResponseDto response = await CreateBlogResponseDto(blog);


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

        public async Task<IEnumerable<ModificationResponseDto>> GetBlogModificationsAsync(Guid blogId)
        {
            var responses =  await _blogUniqueRepository.GetAllBlogModifications(blogId);
            List<ModificationResponseDto> modificationResponseDtos = new List<ModificationResponseDto>();

            foreach (var responseDto in responses)
            {
                ModificationResponseDto modificationResponseDto = new ModificationResponseDto()
                {
                    BlogId = responseDto.BlogId,
                    Description = responseDto.Description,
                    ModificationDate = responseDto.ModificationDate,
                    ModificationId = responseDto.ModificationId,
                    UserId = responseDto.UserId,

                };

                modificationResponseDtos.Add(modificationResponseDto);

            }
            return modificationResponseDtos;
            
        }

  

        public async Task<CategoryResponseDto> GetCategoryAsync(Guid categoryId)
        {
            Category? category = await _categoryRepository.Get(categoryId);

            if (category == null)
            {
                throw new Exception("Category does not exist");
            }

            CategoryResponseDto response = new CategoryResponseDto()
            {
                CategoryId = categoryId,
                CategoryNameEn = category.CatergoryNameEn,
                CategoryNameEs = category.CatergoryNameEs,
            };

            return response;
        }

        public async Task<CommentResponseDto> GetCommentAsync(Guid id)
        {
           Comment? comment = await _commentRepository.Get(id);
            if (comment == null)
            {
                throw new Exception("Comment does not exist");
            }

            CommentResponseDto response = new CommentResponseDto()
            {
                BlogId = comment.BlogId,
                CommentId = comment.CommentId,
                Message = comment.Message,
                UserId = comment.UserId,
                UserName = comment.UserName,

            };

            return response;
        }

        public async Task<KeywordResponseDto> GetKeywordAsync(Guid keywordId)
        {
            Keyword? keyword = await _keywordRepository.Get(keywordId);
            if(keyword == null)
            {
                throw new Exception("Keyword does not exist");
            }

            KeywordResponseDto response = new KeywordResponseDto()
            {
                KeywordId = keywordId,
                KeywordNameEn = keyword.KeywordNameEn,
                KeywordNameEs = keyword.KeywordNameEs,
            };

            return response;
        }

        public async Task<ModificationResponseDto> GetModificationAsync(Guid id)
        {
            Modification? modification = await _modificationRepository.Get(id);

            if(modification == null)
            {
                throw new Exception("Modification does not exist");
            }

            ModificationResponseDto response = new ModificationResponseDto()
            {
                BlogId = modification.BlogId,
                Description = modification.Description,
                ModificationDate = modification.ModificationDate,
                ModificationId = modification.ModificationId,
                UserId = modification.UserId,

            };

            return response;
        }

        public async Task<RatingResponseDto> GetRatingAsync(Guid blogId)
        {
            Rating? rating = await _ratingRepository.Get(blogId);
            if(rating == null)
            {
                throw new Exception("Rating does not exist");
            }
            RatingResponseDto response = new RatingResponseDto()
            {
                BlogId = rating.BlogId,
                UserId= rating.UserId,
                RatingId = rating.RatingId,
                RatingScore = rating.RatingScore,
            };

            return response;
        }


        public async Task<TagResponseDto> GetTagAsync(Guid tagId)
        {
            Tag? tag = await _tagRepository.Get(tagId);
            if (tag == null)
            {
                throw new Exception("Tag does not exist.");
            }

            TagResponseDto response = new TagResponseDto()
            {
                TagId = tag.TagId,
                TagNameEn = tag.TagNameEn,
                TagNameEs = tag.TagNameEs,
            };

            return response;
        }

        public async Task<bool> IsSubscriptionRequiredAsync(Guid blogId)
        {
            return await _blogUniqueRepository.IsSubscriptionRequired(blogId);
        }

        public async Task PublishBlogAsync(Guid id)
        {
            Blog? blog = await _blogRepository.Get(id);
            if (blog == null)
            {
                throw new Exception("Blog does not exist");
            }

            blog.IsPublished = true;

            await _blogRepository.Update(blog);
        }

        public async Task RemoveCategoryFromBlogAsync(Guid blogId, Guid categoryId)
        {
            Blog_Category? blogCategory = await _blogCategoryRepository.Find(a => a.BlogId == blogId && a.CategoryId == categoryId);
            
            if(blogCategory == null)
            {
                throw new Exception("Blog does not contain selected category");
            }

            await _blogCategoryRepository.Delete(blogCategory);
        }

        public async Task RemoveImageAsync(Guid imageId)
        {
            var entity = await _blogImageRepository.Get(imageId);
            if (entity == null)
            {
                throw new Exception("Image does not exist");
            }

            await _blogImageRepository.Delete(entity);
        }

        public async Task RemoveKeywordFromBlogAsync(Guid blogId, Guid keywordId)
        {
            var entity = await _blogKeywordRepository.Find(e => e.BlogId == blogId &&  e.KeywordId == keywordId);
            if(entity == null)
            {
                throw new Exception("Blog does not contain selected keyword");
            }

            await _blogKeywordRepository.Delete(entity);
        }

        public async Task RemoveTagFromBlogAsync(Guid blogId, Guid tagId)
        {
            var entity = await _blogTagRepository.Find(e => e.BlogId == blogId && e.TagId == tagId);
            if (entity == null)
            {
                throw new Exception("Blog does not contain selected tag");
            }

            await _blogTagRepository.Delete(entity);
        }


        ////////////////////////////////////////////////////////////
        ///
        //NEED TO FIGURE OUT THE BEST WAY TO GET ALL THE PREVIEWS THAT HAVE A SPECIFIC  KEYWORD


        public async Task<IEnumerable<PublicBlogListDto>> SearchBlogsWithKeywordAsync(Guid keywordId, int pageIndex, int pageSize)
        {
            IEnumerable<Blog> blogs = await _blogUniqueRepository.GetAllBlogPreviewsByKeyword(keywordId, pageIndex, pageSize);
            
            List<PublicBlogListDto> result = new List<PublicBlogListDto>();

            foreach(Blog blog in blogs)
            {
                if(blog.BlogId == Guid.Empty)
                {
                    continue;
                }

                PublicBlogListDto blogPreviewDto = new PublicBlogListDto()
                {
                    BlogId = blog.BlogId,
                    AverageRating = blog.AverageRating,
                    Author = blog.Author,
                   DatePublished = blog.DatePublished,
                   DescriptionEn = blog.DescriptionEn,
                   DescriptionEs = blog.DescriptionEs,
                   SlugEn = blog.SlugEn,
                   SlugEs = blog.SlugEs,
                   TitleEn = blog.TitleEn,
                   TitleEs = blog.TitleEs,
                };

                result.Add(blogPreviewDto);
            }
            
            return result;
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

        public async Task UnpublishBlogAsync(Guid id)
        {
            Blog? blog = await _blogRepository.Get(id);
            if (blog == null)
            {
                throw new Exception("Blog Id does not exist");
            }
            blog.IsPublished = false;
            await _blogRepository.Update(blog);
        }

        public async Task UpdateBlogAsync(BlogPostPutDto blog)
        {
            var entity = await _blogRepository.Get(blog.BlogId);

            if (entity == null)
            {
                throw new Exception("Blog does not exist");
            }

            entity.TitleEs = blog.TitleEs;
            entity.TitleEn = blog.TitleEn;
            entity.ArticleEn = blog.ArticleEn;
            entity.ArticleEs = blog.ArticleEs;
            entity.SlugEn = SlugGenerator.GenerateSlug(entity.TitleEn ?? "");
            entity.SlugEs = SlugGenerator.GenerateSlug(entity.TitleEs ?? "");
            entity.Author = blog.Author;
            entity.DescriptionEn = blog.DescriptionEn;
            entity.DescriptionEs = blog.DescriptionEs;
            entity.IsSubscriptionRequired = blog.IsSubscriptionRequired == "true" ? true : false;
            entity.IsFeatured = blog.IsFeatured == "true" ? true : false;
            entity.IsPublished = blog.IsPublished == "true" ? true : false;

            await _blogRepository.Update(entity);

            // NEED to make sure that all tags, keywords and categories get updates based on which got removed and added


            // ADD/REMOVE CATEGORIES


            ///////////////////////////////////////////////////
            ///// Get current categories associated with the blog
            var currentCatList = await _blogUniqueRepository.GetAllBlogCategories(blog.BlogId);

            var removeList = currentCatList.Where(c => c != null && blog.CategoriesIds != null && !blog.CategoriesIds.Contains(c.CategoryId.ToString())).Select(c => new Blog_Category()
            {
                BlogId = blog.BlogId,
                CategoryId = c != null ? c.CategoryId : Guid.Empty,
            }).ToList();

            var addList = blog.CategoriesIds != null ?  blog.CategoriesIds.Where(c => !currentCatList.Any(d => d?.CategoryId.ToString() == c)).Select(s => new Blog_Category()
            {
                BlogId = blog.BlogId,
                CategoryId = Guid.Parse(s),
            }).ToList() : null;
            if (removeList.Count > 0)
            {
                await _blogCategoryRepository.RemoveRangeAsync(removeList);
            }

            if (addList != null && addList.Count > 0)
            {
                await _blogCategoryRepository.AddRangeAsync(addList);
            }



            // ADD/REMOVE TAGS 

            var currentTagList = await _blogUniqueRepository.GetAllBlogTags(blog.BlogId);

            var tagsToRemove = currentTagList.Where(tag => tag != null && blog.TagsIds != null && !blog.TagsIds.Contains(tag.TagId.ToString())).Select(k => new Blog_Tag()
            {
                BlogId = blog.BlogId,
                TagId = k != null ? k.TagId : Guid.Empty,
            }).ToList();

            var addTagList = blog.TagsIds != null ? blog.TagsIds.Where(stringId => !currentTagList.Any(tag => tag != null && tag.TagId.ToString() == stringId)).Select(stringId => new Blog_Tag()
            {
                BlogId = blog.BlogId,
                TagId = Guid.Parse(stringId)
            }).ToList() : null;

            if (tagsToRemove.Count > 0)
            {
                await _blogTagRepository.RemoveRangeAsync(tagsToRemove);
            }
            if (addTagList != null && addTagList.Count > 0)
            {
                await _blogTagRepository.AddRangeAsync(addTagList);
            }



            // ADD/REMOVE KEYWORDS (up next)

            var currentKeywordList = await _blogUniqueRepository.GetAllBlogKeywords(blog.BlogId);

            var keywordsToRemove = currentKeywordList.Where(keyword => keyword != null && blog.KeywordsIds != null && !blog.KeywordsIds.Contains(keyword.KeywordId.ToString())).Select(keyword => new Blog_Keyword()
            {
                BlogId = blog.BlogId,
                KeywordId = keyword != null ?  keyword.KeywordId : Guid.Empty
            }).ToList();

            var keywordToAdd = blog.KeywordsIds != null ? blog.KeywordsIds.Where(keywordId => !currentKeywordList.Any(keyword => keyword != null && keyword.KeywordId.ToString() == keywordId)).Select(keywordId => new Blog_Keyword()
            {
                BlogId = blog.BlogId,
                KeywordId = Guid.Parse(keywordId)
            }).ToList() : null;

            if (keywordsToRemove.Count > 0)
            {
                await _blogKeywordRepository.RemoveRangeAsync(keywordsToRemove);
            }
            if (keywordToAdd != null && keywordToAdd.Count > 0)
            {
                await _blogKeywordRepository.AddRangeAsync(keywordToAdd);
            }



        }



        public async Task UpdateBlogModigicationAsync(ModificationPostPutDto modificationPostPutDto)
        {
            Modification? modification = await _modificationRepository.Find(e => e.UserId == modificationPostPutDto.UserId && e.BlogId == modificationPostPutDto.BlogId);
        
            if(modification == null)
            {
                throw new Exception("Modification does not exist");
            }

            modification.Description = modificationPostPutDto.Description;

            await _modificationRepository.Update(modification);

        }

        public async Task UpdateCategory(CategoryPostPutDto categoryPostPutDto)
        {
            Category? category = await _categoryRepository.Get(categoryPostPutDto.CategoryId);

            if(category == null)
            {
                throw new Exception("category does not exist");
            }

            category.CatergoryNameEn = categoryPostPutDto.CategoryNameEn;
            category.CatergoryNameEs = categoryPostPutDto.CategoryNameEs;
            

            await _categoryRepository.Update(category);
        }

        public async Task UpdateCommentAsync(CommentPostPutDto commentPostPutDto)
        {
            Comment? comment = await _commentRepository.Get(commentPostPutDto.CommmentId);
            
            if (comment == null)
            {
                throw new Exception("Comment does not exist");
            }

            comment.Message = commentPostPutDto.Message;

            await _commentRepository.Update(comment);
        }

        public async Task UpdateKeywordAsync(KeywordPostPutDto keywordPostPut)
        {
            Keyword? keyword = await _keywordRepository.Get(keywordPostPut.KeywordId);

            if(keyword == null)
            {
                throw new Exception("Keyword does not exist");
            }

            keyword.KeywordNameEn = keywordPostPut.KeywordNameEn;
            keyword.KeywordNameEs = keywordPostPut.KeywordNameEs;

            await _keywordRepository.Update(keyword);


        }

        public async Task UpdateRatingAsync(RatingPostPutDto ratingPostPutDto)
        {
            Rating? rating = await _ratingRepository.Get(ratingPostPutDto.RatingId);

            if(rating == null)
            {
                throw new Exception("Rating does not exist");
            }

            rating.RatingScore = ratingPostPutDto.RatingScore;
            await _ratingRepository.Update(rating);
        }

        public async Task UpdateTagAsync(TagPostPutDto tagPostPutDto)
        {
            Tag? tag = await _tagRepository.Get(tagPostPutDto.TagId);

            if(tag == null)
            {
                throw new Exception("Tag does not exist.");
            }

            tag.TagNameEn = tagPostPutDto.TagNameEn;
            tag.TagNameEs = tagPostPutDto.TagNameEs;

            await _tagRepository.Update(tag);

        }


        public async Task<BlogPreviewsDto> GetAllAdminBlogPreviews(int pageIndex, string category, List<string> tags)
        {
            int perPage = 50;


            IEnumerable<Blog> blogs = await _blogUniqueRepository.GetAllBlogPreviews(pageIndex, perPage, category, tags);

            List<AdminBlogListDto> responseList = new List<AdminBlogListDto>();


            foreach (Blog blog in blogs)
            {
                AdminBlogListDto dto = new AdminBlogListDto()
                {
                    BlogId = blog.BlogId,
                    UserId = blog.UserId,                    
                    BlogName = blog.TitleEn + "..." + blog.TitleEs,
                    TagsCount = await _blogUniqueRepository.GetBlogTagsCountAsync(blog.BlogId), 
                    Author = blog.Author,
                    CategoryCount = await _blogUniqueRepository.GetBlogCategoryCountAsync(blog.BlogId), 
                    KeywordCount = await _blogUniqueRepository.GetBlogKeywordsCountAsync(blog.BlogId), 
                    ViewCount = blog.ViewCount,
                    AverageRating = blog.AverageRating,
                    Published = blog.IsPublished,
                    Featured = blog.IsFeatured,
                    SubscriptionRerquired = blog.IsSubscriptionRequired,
                    CommentCount = blog.Comments != null ? blog.Comments.Count : 0,
                };


                ApplicationUser? User = await _applicationUserRepository.Get(blog.UserId);

                if (User == null)
                {
                    dto.UserName = "";
                }
                else
                {
                    dto.UserName = User.UserName;
                }

                responseList.Add(dto);
            }

            BlogPreviewsDto responseDto = new BlogPreviewsDto();
            responseDto.AdminBlogList = responseList;

            int blogCount = await _blogUniqueRepository.GetBlogCountAsync();

            responseDto.PageCount =  (int)Math.Ceiling((double)blogCount / perPage);

            responseDto.Categories = (List<Category>)await _categoryRepository.GetAll(1, 1000);

            responseDto.Tags = (List<Tag>)await _tagRepository.GetAll(1, 1000);


            return responseDto;
        }



    }
}

﻿using BlogMaster.Core.Contracts;
using BlogMaster.Core.DTO;
using BlogMaster.Core.Models;
using BlogMaster.Core.Models.Identity;
using BlogMaster.Core.Utilities;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using Stripe;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
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


        public async Task UpdateBlogAveragaRating(string? blogId, decimal? rating)
        {
            if(!string.IsNullOrWhiteSpace(blogId))
            {
                var blog = await _blogRepository.Get(Guid.Parse(blogId));

                if(blog != null)
                {
                    blog.AverageRating = rating;

                    await _blogRepository.Update(blog);

                }


            }

        }


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
                TitleEn = blog.TitleEn,
                DescriptionEn = blog.DescriptionEn,
                SlugEn = blog.SlugEn,
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
                    CreatedDate = image.CreatedDate,
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
                UserName = commentPostPutDto.UserName,
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
                Url = image.Url,
                CreatedDate = DateTime.UtcNow,
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


            //update average rating for when sorting
            decimal? avgRating =  await GetBlogAverageRatingAsync(rating.BlogId);

            if (avgRating != null)
            {
                await UpdateBlogAveragaRating(rating.BlogId.ToString(), avgRating);
            }
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
        private string? RemoveSpacesBetweenTags(string? input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            return Regex.Replace(input, @">\s+<", "><");
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
                ArticleEn = RemoveSpacesBetweenTags(blog.ArticleEn?.Trim()),
                TitleEn = blog.TitleEn?.Trim(),
                DescriptionEn = blog.DescriptionEn?.Trim(),
                SlugEn = !string.IsNullOrEmpty(blog.TitleEn) ? SlugGenerator.GenerateSlug(blog.TitleEn) : null,
                Author = blog.Author?.Trim(),
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
            if(category == null || category.CategoryNameEn == null) 
            { 
                throw new ArgumentNullException("Categoty submitted cannot be mull"); 
            }


            var result = await _categoryRepository.Find(c => c.CatergoryNameEn == category.CategoryNameEn.Trim());

            if(result != null)
            {
                Console.WriteLine("Category name already exists");
                return;
            }

            Category entity = new Category()
            {
                CategoryId = Guid.NewGuid(),
                CatergoryNameEn = category.CategoryNameEn?.Trim(),
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
                Message = comment.Message.Trim(),
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

            var keywordEntity = await _keywordRepository.Find(k =>  k.KeywordNameEn == keyword.KeywordNameEn);

            if(keywordEntity != null)
            {
                Console.WriteLine("This keyword already exists.");
                return;
            }

            Keyword entity = new Keyword()
            {
                KeywordId = Guid.NewGuid(),
                KeywordNameEn = keyword.KeywordNameEn?.Trim(),
            };



            await _keywordRepository.Create(entity);

        }

        public async Task CreateTagAsync(TagPostPutDto tag)
        {
            if(tag == null)
            {
                throw new ArgumentNullException("tag cannot be null");
            }

            if(string.IsNullOrEmpty(tag.TagNameEn))
            {
                throw new ArgumentNullException("tag cannot be null");
            }

            var Tag = await _tagRepository.Find(t => t.TagNameEn ==  tag.TagNameEn.Trim());

            if(Tag != null)
            {
                Console.WriteLine("Tag name already exists.");
                return;
            }


            Tag entity = new Tag()
            {
                TagId = Guid.NewGuid(),
                TagNameEn = tag.TagNameEn?.Trim(),
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

        public async Task DeleteCommentAsync(Guid commentId)
        {
            var entity = await _commentRepository.Get(commentId);
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

            //update average rating for when sorting
            decimal? avgRating = await GetBlogAverageRatingAsync(entity.BlogId);

            if (avgRating != null)
            {
                await UpdateBlogAveragaRating(entity.BlogId.ToString(), avgRating);
            }
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

        public async Task<BlogPreviewsDto?> GetBlogRecomendations(List<CategoryResponseDto> categories, string blogId)
        {
 
            if (categories == null || categories.Count == 0)
            {
                return null;
            }

            List<PublicBlogListDto> publicList = new List<PublicBlogListDto>();

            //need to send nulls
            List<Blog> blogs = (List<Blog>)await _blogUniqueRepository.GetAllBlogPreviews(1, 5, categories[0].CategoryNameEn ?? "", new List<string>(), null, null, false);

            int catCount = 1;
            while (blogs.Count < 5 && catCount < categories.Count)
            {
                var categoryName = categories[catCount].CategoryNameEn ?? "";
                var set = await _blogUniqueRepository.GetAllBlogPreviews(1, 5 - blogs.Count, categoryName, new List<string>(), null, null, false);

                blogs = blogs.Concat(set).ToList();
                catCount++;
            }


            foreach (Blog? preview in blogs)
            {
                if (preview != null && preview.BlogId.ToString() != blogId)
                {
                    PublicBlogListDto blogPreviewDto = new PublicBlogListDto()
                    {
                        BlogId = preview.BlogId,
                        Author = preview.Author,
                        DatePublished = preview.DatePublished,
                        DescriptionEn = preview.DescriptionEn,
                        SlugEn = preview.SlugEn,
                        TitleEn = preview.TitleEn,
                        AverageRating = await GetBlogAverageRatingAsync(preview.BlogId),
                        Subscription = preview.IsSubscriptionRequired

                    };
                    publicList.Add(blogPreviewDto);
                }
            }

            foreach (var blogpreview in publicList)
            {
                var ImageView = await GetFirstBlogImage(blogpreview.BlogId.ToString());

                if (ImageView != null && ImageView.ImageData != null)
                {
                    ImageViewDto imageView = new ImageViewDto();
                    string base64String = Convert.ToBase64String(ImageView.ImageData);

                    imageView.src = $"data:{ImageView.MimeType};base64,{base64String}";
                    imageView.MimeType = ImageView.MimeType;
                    imageView.ImageId = ImageView.BlogImageId;
                    imageView.Filename = ImageView.ImageName;

                    blogpreview.ImageView = imageView;

                }

            }

            foreach (var blogpreview in publicList)
            {
                var ImageView = await GetFirstBlogImage(blogpreview.BlogId.ToString());

                if (ImageView != null && ImageView.ImageData != null)
                {
                    ImageViewDto imageView = new ImageViewDto();
                    string base64String = Convert.ToBase64String(ImageView.ImageData);

                    imageView.src = $"data:{ImageView.MimeType};base64,{base64String}";
                    imageView.MimeType = ImageView.MimeType;
                    imageView.ImageId = ImageView.BlogImageId;
                    imageView.Filename = ImageView.ImageName;

                    blogpreview.ImageView = imageView;

                }

            }

            BlogPreviewsDto response = new BlogPreviewsDto();

            response.publicBlogList = publicList;
            
            return response;
        }


        public async Task<BlogPreviewsDto> GetAllBlogPreviews(int pageIndex, string category, List<string> tags, Dictionary<string, string> filters, string sortBy)
        {
            int pageSize = 50;

            IEnumerable<Blog> blogs = await _blogUniqueRepository.GetAllBlogPreviews(pageIndex, pageSize, category, tags, filters, sortBy, false);

            List<PublicBlogListDto> publicList = new List<PublicBlogListDto>();

            foreach(Blog preview in blogs)
            {
                PublicBlogListDto blogPreviewDto = new PublicBlogListDto()
                {
                    BlogId = preview.BlogId,
                    Author = preview.Author,
                    DatePublished = preview.DatePublished,
                    DescriptionEn = preview.DescriptionEn,
                    SlugEn = preview.SlugEn,
                    TitleEn = preview.TitleEn,
                    AverageRating = await GetBlogAverageRatingAsync(preview.BlogId),
                    Subscription = preview.IsSubscriptionRequired,
                };

                publicList.Add(blogPreviewDto);
            }

            foreach(var blogpreview in publicList)
            {
                var ImageView = await GetFirstBlogImage(blogpreview.BlogId.ToString());

                if (ImageView != null && ImageView.ImageData != null)
                {
                    ImageViewDto imageView = new ImageViewDto();
                    string base64String = Convert.ToBase64String(ImageView.ImageData);

                    imageView.src = $"data:{ImageView.MimeType};base64,{base64String}";
                    imageView.MimeType = ImageView.MimeType;
                    imageView.ImageId = ImageView.BlogImageId;
                    imageView.Filename = ImageView.ImageName;
                    blogpreview.ImageView = imageView;
                }

            }
            BlogPreviewsDto response = new BlogPreviewsDto();

            int blogCount = await _blogUniqueRepository.GetBlogCountAsync();

            response.PageCount = (int)Math.Ceiling((double)blogCount / pageSize);

            response.Categories = (List<Category>)await _categoryRepository.GetAll(1, 1000);

            response.Tags = (List<Tag>)await _tagRepository.GetAll(1, 1000);

            response.SearchField = filters.TryGetValue("search", out string? value) ? value : "";

            response.publicBlogList = publicList;
            return response;

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
            blog.DatePublished = DateTime.UtcNow;

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
                    SlugEn = blog.SlugEn,
                    TitleEn = blog.TitleEn,
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
            blog.DatePublished = null;
            await _blogRepository.Update(blog);
        }

        public async Task UpdateBlogAsync(BlogPostPutDto blog)
        {
            var entity = await _blogRepository.Get(blog.BlogId);

            if (entity == null)
            {
                throw new Exception("Blog does not exist");
            }

            entity.TitleEn = blog.TitleEn?.Trim();
            entity.ArticleEn = RemoveSpacesBetweenTags(blog.ArticleEn?.Trim());
            entity.SlugEn = SlugGenerator.GenerateSlug(entity.TitleEn ?? "");
            entity.Author = blog.Author?.Trim();
            entity.DescriptionEn = blog.DescriptionEn?.Trim();
            entity.IsSubscriptionRequired = blog.IsSubscriptionRequired == "true" ? true : false;
            entity.IsFeatured = blog.IsFeatured == "true" ? true : false;
            entity.IsPublished = blog.IsPublished == "true" ? true : false;
            
            if(entity.DatePublished == null && entity.IsPublished == true)
            {
                entity.DatePublished = DateTime.UtcNow;
            }else if(entity.DatePublished != null && entity.IsPublished == false)
            {
                entity.DatePublished = null;
            }

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

            if(category == null || categoryPostPutDto.CategoryNameEn == null)
            {
                throw new Exception("category does not exist");
            }

            var tempCat = await _categoryRepository.Find(c => c.CatergoryNameEn == categoryPostPutDto.CategoryNameEn.Trim());

            if(tempCat != null)
            {
                Console.WriteLine("Category name already exists.");
                return;
            }


            category.CatergoryNameEn = categoryPostPutDto.CategoryNameEn?.Trim();
            

            await _categoryRepository.Update(category);
        }

        public async Task UpdateCommentAsync(CommentPostPutDto commentPostPutDto)
        {
            Comment? comment = await _commentRepository.Get(commentPostPutDto.CommentId);
            
            if (comment == null)
            {
                throw new Exception("Comment does not exist");
            }

            comment.Message = commentPostPutDto.Message?.Trim();

            await _commentRepository.Update(comment);
        }

        public async Task UpdateKeywordAsync(KeywordPostPutDto keywordPostPut)
        {
            Keyword? keyword = await _keywordRepository.Get(keywordPostPut.KeywordId);

            if(keyword == null || keywordPostPut.KeywordNameEn ==  null)
            {
                throw new Exception("Keyword does not exist");
            }

            var tempKeyword = await _keywordRepository.Find(k => k.KeywordNameEn == keywordPostPut.KeywordNameEn.Trim());

            if(tempKeyword != null)
            {
                Console.WriteLine("Keyword already exists.");
                return;
            }


            keyword.KeywordNameEn = keywordPostPut.KeywordNameEn?.Trim();

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

            //update average rating for when sorting
            decimal? avgRating = await GetBlogAverageRatingAsync(rating.BlogId);

            if (avgRating != null)
            {
                await UpdateBlogAveragaRating(rating.BlogId.ToString(), avgRating);
            }
        }

        public async Task UpdateTagAsync(TagPostPutDto tagPostPutDto)
        {
            Tag? tag = await _tagRepository.Get(tagPostPutDto.TagId);

            if(tag == null || tagPostPutDto.TagNameEn == null)
            {
                throw new Exception("Tag does not exist.");
            }

            var tempTag = await _tagRepository.Find(t => t.TagNameEn == tagPostPutDto.TagNameEn.Trim());

            if (tempTag != null)
            {
                Console.WriteLine("Tag name already exists.");
                return;
            }
            
            tag.TagNameEn = tagPostPutDto.TagNameEn?.Trim();

            await _tagRepository.Update(tag);

        }


        public async Task<BlogPreviewsDto> GetAllAdminBlogPreviews(int pageIndex, string category, List<string> tags, Dictionary<string, string> filters, string sortBy)
        {
            int perPage = 50;


            IEnumerable<Blog> blogs = await _blogUniqueRepository.GetAllBlogPreviews(pageIndex, perPage, category, tags, filters, sortBy, true);

            List<AdminBlogListDto> responseList = new List<AdminBlogListDto>();



            foreach (Blog blog in blogs)
            {
                AdminBlogListDto dto = new AdminBlogListDto()
                {
                    BlogId = blog.BlogId,
                    UserId = blog.UserId,
                    BlogName = blog.TitleEn,
                    TagsCount = await _blogUniqueRepository.GetBlogTagsCountAsync(blog.BlogId),
                    Author = blog.Author,
                    CategoryCount = await _blogUniqueRepository.GetBlogCategoryCountAsync(blog.BlogId),
                    KeywordCount = await _blogUniqueRepository.GetBlogKeywordsCountAsync(blog.BlogId),
                    ViewCount = blog.ViewCount,
                    AverageRating = await GetBlogAverageRatingAsync(blog.BlogId),
                    Published = blog.IsPublished,
                    Featured = blog.IsFeatured,
                    SubscriptionRerquired = blog.IsSubscriptionRequired,
                    CommentCount = await _blogUniqueRepository.GetBlogCommentCountAsync(blog.BlogId),
                    Created = blog.CreatedDate.ToString().Remove(11),
                    PublishedDate = blog.DatePublished != null ? blog.DatePublished?.ToString().Remove(11) : ""

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

            
            if(filters.TryGetValue("published", out string? puvalue))
            {
                if (puvalue == "true")
                {
                    responseDto.IsPublished = true;
                }
                else if (puvalue == "false")
                {
                    responseDto.IsPublished = false;
                }
                else
                {
                    responseDto.IsPublished = null;
                }
            }


            if(filters.TryGetValue("featured", out string? fevalue))
            {
                if (fevalue == "true")
                {
                    responseDto.IsFeatured = true;
                }
                else if (fevalue == "false")
                {
                    responseDto.IsFeatured = false;
                }
                else
                {
                    responseDto.IsFeatured = null;
                }
            }

            if(filters.TryGetValue("subscription", out string? subvalue))
            {
                if (subvalue == "true")
                {
                    responseDto.Subscription = true;
                }else if(subvalue == "false")
                {
                    responseDto.Subscription = false;
                }else
                {
                    responseDto.Subscription = null;
                }
            }


            responseDto.SearchField = filters.TryGetValue("search", out string? searchvalue) ? searchvalue : "";

            int blogCount = await _blogUniqueRepository.GetBlogCountAsync();

            responseDto.PageCount =  (int)Math.Ceiling((double)blogCount / perPage);

            responseDto.Categories = (List<Category>)await _categoryRepository.GetAll(1, 1000);

            responseDto.Tags = (List<Tag>)await _tagRepository.GetAll(1, 1000);

            responseDto.SortBy = sortBy;

            return responseDto;
        }

        public async Task<List<BlogImagesResponseDto>> GetAllBlogImages(string blogId)
        {
            List<BlogImagesResponseDto> imageresponses = new List<BlogImagesResponseDto>();

            IEnumerable<BlogImage?> images = await _blogUniqueRepository.GetAllBlogImages(Guid.Parse(blogId));

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
                    CreatedDate = image.CreatedDate,
                };

                imageresponses.Add(imageResponse);
            }

            return imageresponses.OrderBy(i => i.CreatedDate).ToList();
        }
        public async Task<BlogImagesResponseDto?> GetFirstBlogImage(string blogId)
        {
            List<BlogImagesResponseDto> imageresponses = new List<BlogImagesResponseDto>();

            BlogImage? image = await _blogUniqueRepository.GetFirstBlogImage(blogId);



            if (image == null) return null;

            BlogImagesResponseDto imageResponse = new BlogImagesResponseDto()
            {
                BlogImageId = image.ImageId,
                ImageData = image.ImageData,
                ImageName = image.ImageName,
                MimeType = image.MimeType,
                Url = image.Url,
            };


            return imageResponse;
        }

        public async Task<RatingResponseDto?> GetUserRatingOnBlog(Guid blogId, Guid userId)
        {

            Rating? rating = await _ratingRepository.Find(r => r.UserId == userId && r.BlogId == blogId);

            if(rating == null) return null;

            RatingResponseDto ratingResponse = new RatingResponseDto()
            {
                BlogId = blogId,
                RatingId = rating != null ? rating.RatingId : Guid.Empty,
                RatingScore = rating != null ? rating.RatingScore : 0,
                UserId = userId
            };

            return ratingResponse;
        }

        public async Task<decimal?> GetBlogAverageRatingAsync(Guid blogId)
        {
            var ratings = await _ratingRepository.FindAll(r => r.BlogId == blogId, 0, 1000);

            if (ratings == null || !ratings.Any())
            {
                return null;
            }

            return ratings.Average(r => r?.RatingScore);

        }

    }
}

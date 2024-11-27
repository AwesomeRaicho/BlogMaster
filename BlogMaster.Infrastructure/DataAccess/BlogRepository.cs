using BlogMaster.Core.Contracts;
using BlogMaster.Core.Models;
using BlogMaster.Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BlogMaster.Infrastructure.DataAccess
{
    public class BlogRepository : GeneralRepository<Blog>, IBlogRepository
    {

        public BlogRepository(EntityDbContext context) : base(context) { }

        public async Task<IEnumerable<Blog>> GetAllBlogPreviews(int pageIndex, int pageSize, string category, List<string> tags, Dictionary<string, string>? filters, string? sortBy, bool isAdmin)
        {
            IQueryable<Blog>? query = _context.Blogs;

            if (query == null)
            {
                throw new Exception("No blogs found.");
            }

            if(!isAdmin)
            {
                query = query.Where(blog => blog.IsPublished == true);
            }else
            {
                if(filters != null)
                {
                    //filter: [published], [featured], [subscription]
                    if (filters.TryGetValue("subscription", out string? isSubscription))
                    {
                        if (!string.IsNullOrEmpty(isSubscription))
                        {
                            bool subscription = isSubscription == "true" ? true : false;
                            query = query.Where(blog => blog.IsSubscriptionRequired == subscription);
                        }
                    }

                    if (filters.TryGetValue("published", out string? isPublished))
                    {
                        if (!string.IsNullOrEmpty(isPublished))
                        {
                            bool published = isPublished == "true" ? true : false;
                            query = query.Where(blog => blog.IsPublished == published);

                        }
                    }
                
                    if(filters.TryGetValue("featured", out string? isFeaturedd))
                    {
                        if (!string.IsNullOrEmpty(isFeaturedd))
                        {
                            bool featured = isFeaturedd == "true" ? true : false;
                            query = query.Where (blog => blog.IsFeatured == featured);

                        }
                    }

                }
            }

            //text search field
            
            if(filters != null && filters.TryGetValue("search", out string? searchField))
            {
                if (!string.IsNullOrEmpty(searchField))
                {
                    query = query.Where(blog => blog.TitleEn != null && blog.TitleEn.Contains(searchField));
                }
            }

            // category
            if (!string.IsNullOrEmpty(category))
            {
                query = query
                    .Where(blog => blog.BlogCategories != null && blog.BlogCategories.Any(bc => bc.Category != null && (bc.Category.CatergoryNameEn == category)));
            }

            // tags
            if (tags != null && tags.Any())
            {
                query = query
                    .Where(blog => blog.BlogTags != null && blog.BlogTags.Any(bt => bt.Tag != null && (tags.Contains(bt.Tag.TagNameEn ?? ""))));
            }

            // Sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                var parts = sortBy.Split('_');
                string sortField = parts[0];
                string sortOrder = parts.Length > 1 ? parts[1] : "asc";

                query = sortField.ToLower() switch
                {
                    "views" => sortOrder.ToLower() == "asc"
                        ? query.OrderBy(blog => blog.ViewCount)
                        : query.OrderByDescending(blog => blog.ViewCount),

                    "rating" => sortOrder.ToLower() == "asc"
                        ? query.OrderBy(blog => blog.AverageRating)
                        : query.OrderByDescending(blog => blog.AverageRating),

                    "title" => sortOrder.ToLower() == "asc"
                        ? query.OrderBy(blog => blog.TitleEn ?? "")
                        : query.OrderByDescending(blog => blog.TitleEn ?? ""),

                    "creationdate" => sortOrder.ToLower() == "asc"
                        ? query.OrderBy(blog => blog.CreatedDate)
                        : query.OrderByDescending(blog => blog.CreatedDate),

                    _ => sortOrder.ToLower() == "asc"
                        ? query.OrderBy(blog => blog.CreatedDate)
                        : query.OrderByDescending(blog => blog.CreatedDate) 
                };
            }
            else
            {
                query = query.OrderByDescending(blog => blog.DatePublished);
            }

            // Pagination
            List<Blog> result = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return result;
        }

        public async Task<Blog?> GetBlogBySlug(string slug)
        {

            if (_context.Blogs == null)
            {
                throw new InvalidOperationException("The DbSet Blogs is not available in the database context.");
            }

            var blog = await _context.Blogs.FirstOrDefaultAsync(blog => blog.SlugEn == slug);

            return blog;

        }

        public async Task<IEnumerable<Modification>> GetAllBlogModifications(Guid id)
        {
            if (_context.Modifications == null)
            {
                throw new InvalidOperationException("The DbSet Modifications is not available in the database context.");

            }
            return await _context.Modifications.Where(m => m.BlogId == id).ToListAsync();
        }

        public async Task<IEnumerable<Rating>> GetAllBlogRatings(Guid id)
        {
            if (_context.Ratings == null)
            {
                throw new InvalidOperationException("The DbSet Ratings is not available in the database context.");
            }

            return await _context.Ratings.Where(r => r.BlogId == id).ToListAsync();

        }

        public async Task<IEnumerable<Comment>> GetAllBlogComments(Guid blogId, int pageIndex, int pageSize)
        {
            if (_context.Comments == null) 
            {
                throw new InvalidOperationException("Comment table does not exist");
            }

            return await _context.Comments.Where(b => b.BlogId == blogId)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Rating?> GetUserRatingforBlog(Guid blogId, Guid userId)
        {
            if(_context.Ratings == null)
            {
                throw new InvalidOperationException("Rating table does not exist");

            }
            return await _context.Ratings.FirstOrDefaultAsync(r => r.UserId == userId && r.BlogId == blogId);
        }

        public async Task<BlogImage?> GetFirstBlogImage(string blogGuid)
        {

            return await _context.BlogImages.Where(i => i.BlogId == Guid.Parse(blogGuid)).OrderBy(i => i.CreatedDate).FirstOrDefaultAsync();
            
        }

        public async Task<IEnumerable<Category?>> GetAllBlogCategories(Guid blogId)
        {
            if(_context.BlogCategories == null)
            {
                throw new InvalidOperationException("BlogCategories table does not exist");
            }

            var categories = await _context.BlogCategories.Where(b => b.BlogId == blogId).Select(b => b.Category).ToListAsync();

            return categories;
        }

        //keywords
        public async Task<IEnumerable<Keyword?>> GetAllBlogKeywords(Guid blogId)
        {
            if (_context.BlogKeywords == null)
            {
                throw new InvalidOperationException("BlogKeywords table does not exist");
            }

            var keywords = await _context.BlogKeywords.Where(i => i.BlogId == blogId).Select(i => i.Keyword).ToListAsync();

            return keywords;


        }

        //images
        public async Task<IEnumerable<BlogImage?>> GetAllBlogImages(Guid blogId)
        {
            if(_context.BlogImages == null)
            {
                throw new InvalidOperationException("BlogImages table does not exist");
            }

            var blogImages = await _context.BlogImages.Where(i => i.BlogId == blogId).ToListAsync();

            return blogImages;  


        }

        public async Task<bool> IsSubscriptionRequired(Guid blogId)
        {
            if(_context.Blogs == null)
            {
                throw new InvalidOperationException("Blog table does not exist");
            }


            return  await _context.Blogs.Where(b => b.BlogId == blogId).Select(b => b.IsSubscriptionRequired).SingleOrDefaultAsync();


        }

        public async Task<IEnumerable<Blog>> GetAllBlogPreviewsByKeyword(Guid keywordId, int pageIndex, int pageSize)
        {
            if(_context.BlogKeywords == null)
            {
                throw new InvalidOperationException("BlogKeyword Table does not exist");
            }

            return await _context.BlogKeywords
                .Where(e => e.KeywordId == keywordId)
                .Select(e => new Blog
                {
                    BlogId = e.Blog != null ? e.Blog.BlogId : Guid.Empty,
                    TitleEn = e.Blog != null ? e.Blog.TitleEn : "",
                    DescriptionEn = e.Blog != null ? e.Blog.DescriptionEn : "",
                    SlugEn = e.Blog != null ? e.Blog.SlugEn : "",
                    Author = e.Blog != null ? e.Blog.Author : "", 
                    DatePublished = e.Blog != null ? e.Blog.DatePublished : DateTime.UtcNow,
                    AverageRating = e.Blog != null ? e.Blog.AverageRating : 0,
                }
                )
                .Skip((pageIndex -1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetBlogCountAsync()
        {
            if(_context.Blogs != null)
            {
                return await _context.Blogs.CountAsync();
            }
            throw new Exception("Blogs Table does not exist");
        }

        public async Task<IEnumerable<Tag?>> GetAllBlogTags(Guid blogId)
        {
            if (_context.BlogTags == null)
            {
                throw new InvalidOperationException("BlogTags table does not exist");
            }

            return await _context.BlogTags
                .Where(bt => bt.BlogId == blogId)
                .Select(bt => bt.Tag)
                .ToListAsync();
        }

        public async Task<int> GetBlogCategoryCountAsync(Guid blogId)
        {
            if (_context.BlogCategories == null)
            {
                throw new Exception("BlogCategories table does not exist");
            }
            int count = await _context.BlogCategories.CountAsync(i => i.BlogId == blogId);
            return count;
        }

        public async Task<int> GetBlogKeywordsCountAsync(Guid blogId)
        {
            if (_context.BlogKeywords == null)
            {
                throw new Exception("BlogKeywords table does not exist");
            }
            int count = await _context.BlogKeywords.CountAsync(i => i.BlogId == blogId);
            return count;
        }

        public async Task<int> GetBlogTagsCountAsync(Guid blogId)
        {
            if( _context.BlogTags == null)
            {
                throw new Exception("BlogTags table does not exist");
            }
            int Count = await _context.BlogTags.CountAsync(i => i.BlogId == blogId);
            return Count;
        }

        public async Task<int> GetBlogCommentCountAsync(Guid blogId)
        {
            if (_context.Comments == null)
            {
                throw new Exception("Comments table does not exist");
            }
            int count = await _context.Comments.CountAsync(i => i.BlogId == blogId);
            return count;
        }
    }
}

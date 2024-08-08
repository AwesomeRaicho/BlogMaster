using BlogMaster.Core.Contracts;
using BlogMaster.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BlogMaster.Infrastructure.DataAccess
{
    public class BlogRepository : GeneralRepository<Blog>, IBlogRepository
    {


        public BlogRepository(EntityDbContext context) : base(context) { }



        public async Task<IEnumerable<Blog>> GetAllBlogPreviews(int pageIndex, int pageSize, string category, List<string> tags)
        {

            IQueryable<Blog>? query = _context.Blogs;

            if (query == null)
            {
                throw new Exception("No blogs found.");
            }

            // category
            if (!string.IsNullOrEmpty(category))
            {
                query = query
                    .Where(blog => blog.BlogCategories != null && blog.BlogCategories.Any(bc => bc.Category != null && (bc.Category.CatergoryNameEn == category || bc.Category.CatergoryNameEs == category)));
            }

            // tags
            if (tags != null && tags.Any())
            {
                query = query
                    .Where(blog => blog.BlogTags != null && blog.BlogTags.Any(bt => bt.Tag != null && (tags.Contains(bt.Tag.TagNameEn ?? "") || tags.Contains(bt.Tag.TagNameEs ?? ""))));

            }

            // pagination
            var result = await query
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

            return await _context.Blogs.FirstOrDefaultAsync(blog =>  (blog.ArticleEn == slug || blog.ArticleEs == slug));

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

    }
}

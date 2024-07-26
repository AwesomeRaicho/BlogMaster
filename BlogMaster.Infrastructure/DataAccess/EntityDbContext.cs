using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogMaster.Core.Models;

namespace BlogMaster.Infrastructure.DataAccess
{
    public class EntityDbContext : DbContext
    {
        //public DbSet<Item>? Items { get; set; }
        //public DbSet<ItemOption>? ItemOptions { get; set; }

        public DbSet<Blog>? Blogs { get; set; }
        public DbSet<Blog_Category>? BlogCategories { get; set; }
        public DbSet<Blog_Keyword>? BlogKeywords { get; set; }
        public DbSet<Blog_Tag>? BlogTags { get; set; }
        public DbSet<Category>? Categories { get; set; }
        public DbSet<Comment>? Comments { get; set; }
        public DbSet<Keyword>? Keywords { get; set; }
        public DbSet<Modification>? Modifications { get; set; }
        public DbSet<Rating>? Ratings { get; set; }
        public DbSet<Tag>? Tags { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // RELATIONSHIPS 
            // Blog & Categorie = Blog_Category
            modelBuilder.Entity<Blog_Category>()
                .HasKey(e => new { e.BlogId, e.CategoryId });

            modelBuilder.Entity<Blog_Category>()
                .HasOne(e => e.Blog)
                .WithMany(e => e.BlogCategories)
                .HasForeignKey(e => e.BlogId);

            modelBuilder.Entity<Blog_Category>()
                .HasOne(e => e.Category)
                .WithMany(e => e.BlogCategories)
                .HasForeignKey(e => e.CategoryId);

            modelBuilder.Entity<Blog>()
                .HasMany(e => e.BlogCategories)
                .WithOne(e => e.Blog)
                .HasForeignKey(e => e.BlogId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>()
                .HasMany(e => e.BlogCategories)
                .WithOne(e => e.Category)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            //Blog & Keyword = Blog_Keyword
            modelBuilder.Entity<Blog_Keyword>()
                .HasKey(e => new { e.BlogId, e.KeywordId });

            modelBuilder.Entity<Blog_Keyword>().
                HasOne(e => e.Blog)
                .WithMany(e => e.BlogKeywords)
                .HasForeignKey(e => e.BlogId);

            modelBuilder.Entity<Blog_Keyword>()
                .HasOne(e => e.Keyword)
                .WithMany(e => e.BlogKeywords)
                .HasForeignKey(e => e.KeywordId);

            modelBuilder.Entity<Blog>()
                .HasMany(e => e.BlogKeywords)
                .WithOne(e => e.Blog)
                .HasForeignKey(e => e.BlogId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Keyword>()
                .HasMany(e => e.BlogKeywords)
                .WithOne(e => e.Keyword)
                .HasForeignKey(e => e.KeywordId)
                .OnDelete(DeleteBehavior.Cascade);

            //Blog & Tag = Blog_Tag
            modelBuilder.Entity<Blog_Tag>()
                .HasKey(e => new {e.BlogId, e.TagId});

            modelBuilder.Entity<Blog_Tag>()
                .HasOne(e => e.Blog)
                .WithMany(e => e.BlogTags)
                .HasForeignKey(e => e.BlogId);

            modelBuilder.Entity<Blog_Tag>()
                .HasOne(e => e.Tag)
                .WithMany(e => e.BlogTags)
                .HasForeignKey(e => e.TagId);

            modelBuilder.Entity<Blog>()
                .HasMany(e => e.BlogTags)
                .WithOne(e => e.Blog)
                .HasForeignKey(e => e.BlogId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tag>()
                .HasMany(e => e.BlogTags)
                .WithOne(e => e.Tag)
                .HasForeignKey(e => e.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            //END OF RELATIONSHIPS






            base.OnModelCreating(modelBuilder);

        }


    }

}

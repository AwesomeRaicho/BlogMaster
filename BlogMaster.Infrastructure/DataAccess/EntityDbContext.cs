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
            // RELATIONSHIPS MANY-TO-MANY
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



            //END OF RELATIONSHIPS MANY-TO-MANY



            // <BLOG> 
            modelBuilder.Entity<Blog>()
                .HasIndex(e => e.SlugEn)
                .HasDatabaseName("IX_Blog_SlugEn");

            modelBuilder.Entity<Blog>()
                .HasIndex(e => e.SlugEs)
                .HasDatabaseName("IX_Blog_SlugEs");

            modelBuilder.Entity<Blog>()
            .Property(b => b.SlugEn)
            .HasMaxLength(255);

            modelBuilder.Entity<Blog>()
                .Property(b => b.SlugEs)
                .HasMaxLength(255);

            modelBuilder.Entity<Blog>()
                .Property(e => e.AverageRating)
                .HasPrecision(2, 1);

            modelBuilder.Entity<Blog>()
                .Property(e => e.TitleEn)
                .HasMaxLength(255);  

            modelBuilder.Entity<Blog>()
                .Property(e => e.TitleEs)
                .HasMaxLength(255);  

            modelBuilder.Entity<Blog>()
                .Property(e => e.DescriptionEn)
                .HasMaxLength(2000);  

            modelBuilder.Entity<Blog>()
                .Property(e => e.DescriptionEs)
                .HasMaxLength(2000);

            modelBuilder.Entity<Blog>()
                .Property(e => e.Author)
                .HasMaxLength(100);


            // <BLOG> Relation

            modelBuilder.Entity<Blog>()
            .HasMany(e => e.BlogImages)
            .WithOne(e => e.Blog)
            .HasForeignKey(e => e.ImageId)
            .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Blog>()
            .HasMany(e => e.Comments)
            .WithOne(e => e.Blog)
            .HasForeignKey(e => e.CommentId)
            .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Blog>()
            .HasMany(e => e.Ratings)
            .WithOne(e => e.Blog)
            .HasForeignKey(e => e.RatingId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Blog>()
            .HasMany(e => e.Modifications)
            .WithOne(e => e.Blog)
            .HasForeignKey(e => e.ModificationId)
            .OnDelete(DeleteBehavior.Cascade);


            // <BLOG> end


            // <BLOGIMAGE>
            modelBuilder.Entity<BlogImage>()
            .HasKey(e => e.ImageId);

            modelBuilder.Entity<BlogImage>()
                .Property(e => e.ImageName)
                .HasMaxLength(100);

            modelBuilder.Entity<BlogImage>()
                .Property(e => e.MimeType)
                .HasMaxLength(15);

            modelBuilder.Entity<BlogImage>()
                .Property(e => e.Url)
                .HasMaxLength(500);

            //<BLOGIMAGE> Relation
            modelBuilder.Entity<BlogImage>()
                .HasOne(e => e.Blog)
                .WithMany(e => e.BlogImages)
                .HasForeignKey(e => e.BlogId);


            // <BLOGIMAGE> end


            //<CATEGORY>

            modelBuilder.Entity<Category>()
                .HasKey(e => e.CategoryId);

            modelBuilder.Entity<Category>()
                .Property(e => e.CatergoryNameEn)
                .HasMaxLength(50);

            modelBuilder.Entity<Category>()
                .Property(e => e.CatergoryNameEs)
                .HasMaxLength(50);

            //<CATEGORY> end

            //<COMMENT>
            modelBuilder.Entity<Comment>()
                .HasKey(e => e.CommentId);

            modelBuilder.Entity<Comment>()
                .Property(e => e.UserName)
                .HasMaxLength(20);

            modelBuilder.Entity<Comment>()
                .Property(e => e.Message)
                .HasMaxLength(500);

            //<COMMENT> Relation
            modelBuilder.Entity<Comment>()
                .HasOne(e => e.Blog)
                .WithMany(e => e.Comments)
                .HasForeignKey(e => e.BlogId);

            //<CATEGORY> end

            //<KEYWORD>

            modelBuilder.Entity<Keyword>()
                .HasKey(e => e.KeywordId);

            modelBuilder.Entity<Keyword>()
                .Property(e => e.KeywordNameEn)
                .HasMaxLength(15);

            modelBuilder.Entity<Keyword>()
                .Property(e => e.KeywordNameEs)
                .HasMaxLength(15);

            //<KEYWORD> end

            //<MODIFICATION> 

            modelBuilder.Entity<Modification>()
                .HasKey (e => e.ModificationId);

            modelBuilder.Entity<Modification>()
                .Property(e => e.UserName)
                .HasMaxLength(20);

            modelBuilder.Entity<Modification>()
                .Property(e => e.Description)
                .HasMaxLength(300);


            //<MODIFICATION> relation

            modelBuilder.Entity<Modification>()
                .HasOne(e => e.Blog)
                .WithMany(e => e.Modifications)
                .HasForeignKey(e => e.BlogId);

            //<MODIFICATION> end

            //<RATING> end


            modelBuilder.Entity<Rating>()
                .HasKey(e => e.RatingId);

            modelBuilder.Entity<Rating>()
                .Property(e => e.UserName)
                .HasMaxLength(20);

            modelBuilder.Entity<Rating>()
                .Property(e => e.RatingScore)
                .HasPrecision(2, 1);

            //<RATING> relation

            modelBuilder.Entity<Rating>()
                .HasOne(e => e.Blog)
                .WithMany(e => e.Ratings)
                .HasForeignKey (e => e.BlogId);

            //<RATING> end

            //<TAG>

            modelBuilder.Entity<Tag>()
                .HasKey(e => e.TagId);

            modelBuilder.Entity<Tag>()
                .Property(e => e.TagNameEn)
                .HasMaxLength(15);

            modelBuilder.Entity<Tag>()
                .Property(e => e.TagNameEs)
                .HasMaxLength(15);


            //<TAG> end



            base.OnModelCreating(modelBuilder);

        }


    }

}

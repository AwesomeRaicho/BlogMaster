using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class AdminBlogListDto
    {
        public Guid BlogId { get; set; }
        public Guid UserId { get; set; }
        public string? BlogName { get; set; }
        public string? UserName { get; set; }
        public string? Author { get; set; }
        public int TagsCount { get; set; }
        public int CategoryCount { get; set; }
        public bool? Published {  get; set; } 
        public bool? Featured {  get; set; }
        public int? ViewCount { get; set; }
        public decimal? AverageRating { get; set; }
        public bool SubscriptionRerquired { get; set; }
        public int CommentCount { get; set; }

    }
}

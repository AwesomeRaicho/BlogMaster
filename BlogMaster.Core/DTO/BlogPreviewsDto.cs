using BlogMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class BlogPreviewsDto
    {
        public int PageCount { get; set; }
        public List<AdminBlogListDto>? AdminBlogList { get; set; }

        public List<PublicBlogListDto>? publicBlogList { get; set; }

        

        public string? SearchField { get; set; }
        public string? SortBy { get; set; }
        public string? Order {  get; set; }
        public bool? Subscription {  get; set; }
        public bool? IsFeatured { get; set; }
        public bool? IsPublished { get; set; }

        public List<Category>? Categories { get; set; } = new List<Category>();
        public List<Tag>? Tags { get; set; } = new List<Tag>();
    }
}

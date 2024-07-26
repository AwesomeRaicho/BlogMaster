using BlogMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class BlogResponseDto
    {
        public Guid BlogId { get; set; }
        public string? Article { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Author { get; set; }
        public DateTime DatePublished { get; set; }
        public string? Category { get; set; }
        public decimal? Rating { get; set; }
        public List<Comment>? Comments { get; set; }
    }
}

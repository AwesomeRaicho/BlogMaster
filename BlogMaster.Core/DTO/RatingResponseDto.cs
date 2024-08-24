using BlogMaster.Core.Models.Identity;
using BlogMaster.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class RatingResponseDto
    {
        public Guid RatingId { get; set; }
        public decimal? RatingScore { get; set; }

        public Guid BlogId { get; set; }
        public Guid UserId { get; set; }


    }
}

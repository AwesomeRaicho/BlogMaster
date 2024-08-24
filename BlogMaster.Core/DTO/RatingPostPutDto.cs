using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class RatingPostPutDto
    {
        public Guid RatingId {  get; set; }
        public Guid BlogId { get; set; }
        public Guid UserId { get; set; }
    
        
        [Required(ErrorMessage = "Rating Score Required")]
        [Range(0.5, 5)]
        public decimal? RatingScore { get; set; }
    
    
    }
}

using Stripe;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class NewPaymentMethodDto
    {
        [Required]
        public string? Number { get; set; }
        [Required]
        public int ExpMonth { get; set; }
        [Required]
        public int ExpYear { get; set; }
        [Required]
        public string? Cvc { get; set; }

        public string? UserId { get; set; }


    }
}

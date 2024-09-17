using BlogMaster.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Models
{
    public class AppSubscription
    {
        [Key]
        public Guid AppSubscriptionId { get; set; }
        public  Guid UserId { get; set; }
        public string? CustomerId { get; set; }
        public string? SubscriptionId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        
        public string? Status {  get; set; }
        
        public DateTime? CancelationDate { get; set; }
        public DateTime? NextBillingDate { get; set; }



        // Navigation property
        public ApplicationUser? User { get; set; }
    }
}

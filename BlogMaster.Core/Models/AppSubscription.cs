using BlogMaster.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Models
{
    public class AppSubscription
    {
        public string? UserId { get; set; }
        public string? Status {  get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CancelationDate { get; set; }
        public string? CustomerId { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? NextBillingDate { get; set; }
        public string? SubscriptionId { get; set; }



        // Navigation property
        public ApplicationUser? User { get; set; }
    }
}

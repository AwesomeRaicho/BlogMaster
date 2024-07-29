using BlogMaster.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Models
{
    public class Subscription
    {
        public Guid SubscriptionId { get; set; }
        public bool IsSubscribed { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

         

        public Guid UserId { get; set; }

        // Navigation property
        public ApplicationUser? User { get; set; }
    }
}

using BlogMaster.Core.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Models
{
    public class PaymentHistory
    {
        public Guid PaymentId { get; set; }
        public string? CustomerId { get; set; }
        public string? UserId { get; set; }
        public string? SubscriptionId { get; set; }
        public DateTime PaymentDate { get; set; }
        public long? Amount { get; set; }
        public string? PaymentStatus { get; set; }
        public string? SessionId { get; set; }

        //Navigation 
        public ApplicationUser? User { get; set; }



    }
}

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
        public Guid UserId { get; set; }
        public Guid SubscriptionId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public Guid TransactionId { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? CreatedDate { get; set; }

        //Navigation 
        public ApplicationUser? User { get; set; }
        public Subscription? subscription { get; set; }

    }
}

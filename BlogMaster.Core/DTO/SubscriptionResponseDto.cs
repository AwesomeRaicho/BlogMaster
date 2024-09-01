using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class SubscriptionResponseDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Status { get; set; }
        public DateTime? CancelationDate { get; set; }
        public DateTime NextBillingDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

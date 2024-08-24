using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Models
{
    public class PaymentMethod
    {
        public Guid PaymentMethodId { get; set; }
        public Guid UserId { get; set; }
        public string? PaymentMethodType { get; set;}
        
    }
}

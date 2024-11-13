using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.DTO
{
    public class CardPaymentMethodDTO
    {
        public string? Cvc {  get; set; }
        public string? Number { get; set; }
        public long ExpMonth { get; set; }
        public long ExpYear { get; set; }
        public string? Name { get; set; }
        public string? StripeCustomerId { get; set; }
        public string? DefaultMethod { get; set; }
    }
}

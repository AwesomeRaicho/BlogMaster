using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stripe.Checkout;


namespace BlogMaster.Core.DTO
{
    public class SessionReturnDto
    {
        public Session? StripeSession { get; set; }
        public string? RedirectController { get; set; }
        public string? RedirectMethod { get; set; }
        public SubscriptionResponseDto? SubscriptionResponseDto { get; set; }
    }
}

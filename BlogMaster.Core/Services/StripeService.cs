using BlogMaster.Core.Models;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Services
{
    public class StripeService
    {
        private readonly StripeSettings _stripeSettings;

        public StripeService(StripeSettings stripeSettings)
        {
            _stripeSettings = stripeSettings;
        }

        public void test1()
        {
            StripeConfiguration.ApiKey = _stripeSettings.PrivateKey;

            var options = new SessionCreateOptions()
            {
                SuccessUrl = "https://localhost:7218/successcheckout",
                CancelUrl = "https://localhost:7218/cancelled",
                LineItems = new List<SessionLineItemOptions>()
                {
                    new SessionLineItemOptions()
                    {
                        Price = "price_1MotwRLkdIwHu7ixYcPLm5uZ",
                        Quantity = 2,
                    },
                },
                Mode = "payment",
                CustomerEmail = "ricardo.araujo0188@gmail.com",
            };
            var service = new SessionService();
            service.Create(options);
        }
    }
}

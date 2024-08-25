using BlogMaster.Core.Models;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogMaster.Core.Contracts;
using Microsoft.Extensions.Options;

namespace BlogMaster.Core.Services
{
    public class StripeService : IStripeService
    {
        private readonly StripeSettings _stripeSettings;

        public StripeService(IOptions<StripeSettings> stripeSettings)
        {
            _stripeSettings = stripeSettings.Value ;
        }

        public Session StartSessionForEmbededForm(GetFormRequestDto getFormRequestDto)
        {

            try
            {

                var options = new SessionCreateOptions
                {
                    LineItems = new List<SessionLineItemOptions>
                    {
                      new SessionLineItemOptions
                      {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                          UnitAmount = 2000,
                          Currency = "mxn",
                          ProductData = new SessionLineItemPriceDataProductDataOptions
                          {
                            Name = "Donation",
                          },
                        },
                        Quantity = 1,
                      },
                    },
                    Mode = "payment",
                    UiMode = "embedded",
                    ReturnUrl = "https://example.com/return?session_id={CHECKOUT_SESSION_ID}",
                };

                var service = new SessionService();
                return service.Create(options);
            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw new Exception(ex.ToString());
            }
        }


        public string? GetPublishableKey()
        {
            string? something = _stripeSettings.PublishableKey;

            return _stripeSettings.PublishableKey;
        }


    }
}

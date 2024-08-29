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
using BlogMaster.Core.Models.Identity;
using BlogMaster.Core.DTO;

namespace BlogMaster.Core.Services
{
    public class StripeService : IStripeService
    {


        private readonly string DomainName = "https://localhost:7218";
        private readonly StripeSettings _stripeSettings;
        private readonly IIdentityService _identityService;

        public StripeService(IOptions<StripeSettings> stripeSettings, IIdentityService identityService)
        {
            _stripeSettings = stripeSettings.Value;
            _identityService = identityService;
        }

        //Customer Methods
        public Customer CreateStripeCustomer(AppSubscription appSubscription)
        {
            try
            {
                var options = new CustomerCreateOptions
                {
                    Name = appSubscription.UserName,
                    Email = appSubscription.UserEmail,
                };
                var service = new CustomerService();
                return service.Create(options);

            }catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new Exception(ex.Message);
            }
        }




        ////////////////////////////////////////////////////////////////




        public Task<Session> StartSessionForEmbededFormDonation(GetFormRequestDto getFormRequestDto)
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
                          UnitAmount = getFormRequestDto.Amount * 1000,
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
                    ReturnUrl = $"{DomainName}/payment-return?session_id={{CHECKOUT_SESSION_ID}}",
                };

                var service = new SessionService();
                return service.CreateAsync(options);
            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw new Exception(ex.ToString());
            }
        }

        public Session StartSessionForEmbededFormSubscription(GetFormRequestDto getFormRequestDto)
        {

            try
            {

                var options = new SessionCreateOptions
                {
                    Customer = "Something in the way",
                    LineItems = new List<SessionLineItemOptions>
                    {
                      new SessionLineItemOptions
                        {
                            Price = "price_1Prtw409TbzP0h4ikMqH8DnJ", 
                            Quantity = 1,
                        },
                    },
                    Mode = "subscription",
                    UiMode = "embedded",
                    ReturnUrl = $"{DomainName}/payment-return?session_id={{CHECKOUT_SESSION_ID}}",
                };

                var service = new SessionService();
                return service.Create(options);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw new Exception(ex.ToString());
            }
        }

        /// <summary>
        /// ////////////////////
        /// </summary>
        /// <returns></returns>
        public string? GetPublishableKey()
        {
            string? something = _stripeSettings.PublishableKey;

            return _stripeSettings.PublishableKey;
        }

        public void CancelSubscription(string subId)
        {
            var options = new SubscriptionUpdateOptions()
            { 
                CancelAtPeriodEnd = true 
            };

            var service = new SubscriptionService();
            service.Update(subId, options);

        }
    }
}

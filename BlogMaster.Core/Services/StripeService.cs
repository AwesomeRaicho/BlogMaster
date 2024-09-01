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
        public Customer CreateStripeCustomer(string userName, string userEmail)
        {
            try
            {
                //check if user exists in Stripe
                var customerSearchOptions = new CustomerSearchOptions
                {
                    Query = $"name:'{userName}' AND email:'{userEmail}'",
                };
                var CustomerService = new CustomerService();
                var data = CustomerService.Search(customerSearchOptions);



                if(data.Data == null)
                {
                    var options = new CustomerCreateOptions
                    {
                        Name = userName,
                        Email = userEmail,
                    };
                    var service = new CustomerService();
                    return service.Create(options);
                }
                else
                {
                    if(data.Data.Count() == 1)
                    {
                        return data.Data[0];
                    }

                    throw new Exception("multiple users with the same name in Stripe");
                }



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
                          UnitAmount = getFormRequestDto.Amount * 100,
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

        public Task<Session> StartSessionForEmbededFormSubscription(GetFormRequestDto getFormRequestDto)
        {
            if(getFormRequestDto == null || string.IsNullOrEmpty(getFormRequestDto.Username) || string.IsNullOrEmpty(getFormRequestDto.UserEmail))
            {
                throw new ArgumentNullException(nameof(getFormRequestDto));
            }


            
            // Get or create a customer for the subscription session
            Customer? customer = this.CreateStripeCustomer(getFormRequestDto.Username, getFormRequestDto.UserEmail);



            try
            {

                var sessionCreatedOptions = new SessionCreateOptions
                {

                    Customer = $"{customer.Id}", 
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

                var sessionService = new SessionService();
                return sessionService.CreateAsync(sessionCreatedOptions);
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

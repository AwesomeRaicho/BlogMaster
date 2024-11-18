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
using Microsoft.Extensions.Configuration;

namespace BlogMaster.Core.Services
{
    public class StripeService : IStripeService
    {

        private readonly string DomainName;
        private readonly string PriceId = "price_1Prtw409TbzP0h4ikMqH8DnJ";
        private readonly StripeSettings _stripeSettings;
        private readonly IIdentityService _identityService;

        public StripeService(IOptions<StripeSettings> stripeSettings, IIdentityService identityService, IConfiguration configuration)
        {
            _stripeSettings = stripeSettings.Value;
            _identityService = identityService;
            DomainName = configuration["Domain:DomainName"] ?? "";
        }

        //Customer Methods

        /// <summary>
        /// This method will create a user on Stripe or it will return the customer if it alredy exists (looks for matching userName and Email )
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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




                if(data.Data.Count == 0)
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

        public async Task<SessionReturnDto> StartSessionForEmbededFormSubscription(GetFormRequestDto getFormRequestDto)
        {
            if(getFormRequestDto == null || string.IsNullOrEmpty(getFormRequestDto.UserName) || string.IsNullOrEmpty(getFormRequestDto.UserEmail))
            {
                throw new ArgumentNullException(nameof(getFormRequestDto));
            }

            SessionReturnDto sessionReturnDto = new SessionReturnDto() 
            {

            };

            // Get or create a customer for the subscription session
            Customer? customer =  this.CreateStripeCustomer(getFormRequestDto.UserName, getFormRequestDto.UserEmail);

            try
            {

                var sessionCreatedOptions = new SessionCreateOptions
                {
                    Customer = $"{customer.Id}",
                    PaymentMethodTypes = new List<string> { "card" },
                    SubscriptionData =
                    {
                        
                    },
                    LineItems = new List<SessionLineItemOptions>
                    {
                      new SessionLineItemOptions
                        {
                            Price = PriceId, 
                            Quantity = 1,
                            
                        },
                      
                    },
                    Mode = "subscription",
                    UiMode = "embedded",
                    ReturnUrl = $"{DomainName}/payment-return?session_id={{CHECKOUT_SESSION_ID}}",
                };

                var sessionService = new SessionService();

                sessionReturnDto.StripeSession = sessionService.Create(sessionCreatedOptions);

                return sessionReturnDto;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw new Exception(ex.ToString());
            }
        }



        /// <summary>
        /// Provides you the key that can be used to request the embedded form in the front end "public key"
        /// </summary>
        /// <returns></returns>
        public string? GetPublishableKey()
        {
            string? something = _stripeSettings.PublishableKey;

            return _stripeSettings.PublishableKey;
        }

        public async Task<Subscription?> GetCustomerSubscription(string customerId)
        {
            var options = new SubscriptionListOptions
            {
                Customer = customerId,
                
            };

            var service = new SubscriptionService();
            StripeList<Subscription> subscriptions = await service.ListAsync(options);

            if (subscriptions.Data != null)
            {
                if (subscriptions.Data.Count > 1)
                {
                    throw new Exception("There are more than 2 subscription for this user");
                }

                if(subscriptions.Data.Count == 0)
                {
                    return null;
                }

                return subscriptions.Data[0] as Subscription;

            }

            return null;
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



        /// <summary>
        /// Possible values are incomplete, incomplete_expired, trialing, active, past_due, canceled, unpaid, or paused.
        /// </summary>
        /// <param name="stripeCustomerId"></param>
        /// <returns></returns>
        public async Task<string?> SubscriptionStatus(string stripeCustomerId)
        {
            Subscription? subscription = await GetCustomerSubscription(stripeCustomerId);

            if (subscription == null)
            {
                return null;
            }

            return subscription.Status;

        }

        public bool ResumeSubscription(string subscriptionId)
        {

            var options = new SubscriptionUpdateOptions()
            {
                CancelAtPeriodEnd = false
            };

            var service = new SubscriptionService();
            service.Update(subscriptionId, options);
            return false;
        }

        public StripeList<PaymentMethod> StripePaymentMethods(string stripeCustomerId)
        {

            var options = new PaymentMethodListOptions
            {
                Type = "card",
                Customer = stripeCustomerId,
                Limit = 10
            };
            var service = new PaymentMethodService();
            StripeList<PaymentMethod> paymentMethods = service.List(options);

            return paymentMethods;

        }



        public void RemovePaymentMethod(string paymentMethodId)
        {
            var service = new PaymentMethodService();
            service.Detach(paymentMethodId);
        }

        public void ChangeDefaultPaymentMethod(string customerId, string paymentMethodId, string subscriptionId)
        {

            var customerOptions = new CustomerUpdateOptions
            {
                InvoiceSettings = new CustomerInvoiceSettingsOptions()
                {
                    DefaultPaymentMethod = paymentMethodId
                },
                
            };

            var customerService = new CustomerService();
            customerService.Update(customerId, customerOptions);

            var subscriptionOptions = new SubscriptionUpdateOptions
            {
                DefaultPaymentMethod = paymentMethodId,
            };

            var subdcriptionService = new SubscriptionService();
            subdcriptionService.Update(subscriptionId, subscriptionOptions);

        }

        public void CreatePaymentMethod(CardPaymentMethodDTO cardPaymentMethod)
        {
            try
            {
                var options = new PaymentMethodCreateOptions
                {
                    Type = "card",
                    Card = new PaymentMethodCardOptions
                    {
                        Cvc = cardPaymentMethod.Cvc,
                        Number = cardPaymentMethod.Number,
                        ExpMonth = cardPaymentMethod.ExpMonth,
                        ExpYear = cardPaymentMethod.ExpYear
                    },
                    BillingDetails = new PaymentMethodBillingDetailsOptions
                    {
                        Name = cardPaymentMethod.Name,
                    }
                };

                var paymentMethodService = new PaymentMethodService();
                var paymentMethod = paymentMethodService.Create(options);

                var attachOptions = new PaymentMethodAttachOptions
                {
                    Customer = cardPaymentMethod.StripeCustomerId
                };

                paymentMethodService.Attach(paymentMethod.Id, attachOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating payment method: {ex.Message}");
            }
        }

        public Session StartSessionForEmbededSession(string customerId, string? email)
        {

            var options = new SessionCreateOptions
            {
                Currency = "mxn",
                Mode = "setup",
                UiMode = "embedded",
                Customer = customerId,
                ReturnUrl = $"{DomainName}/payment-return?session_id={{CHECKOUT_SESSION_ID}}",

            };



            var service = new SessionService();
            return service.Create(options);
        }
        public SetupIntent StartSessionForEmbededIntent(string customerId, string? email)
        {
            var options = new SetupIntentCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Customer = customerId,
            };
            var service = new SetupIntentService();
            return service.Create(options);

        }
    }
}

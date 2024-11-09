using BlogMaster.Core.Contracts;
using BlogMaster.Core.DTO;
using BlogMaster.Core.Models;
using BlogMaster.Core.Models.Identity;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Services
{
    /// <summary>
    /// Service in charge of handeling subscriptions, payment methods, payment history and payment process.
    /// </summary>
    public class AppSubscriptionService : IAppSubscriptionService
    {
        private readonly IStripeService _stripeService;
        private readonly IIdentityService _identityService;
        private readonly IRepository<AppSubscription> _subscriptionRepository;


        public AppSubscriptionService(IStripeService stripeService, IIdentityService identityService, IRepository<AppSubscription> subscriptionRepository)
        {
            _stripeService = stripeService;
            _identityService = identityService;
            _subscriptionRepository = subscriptionRepository;
        }

        private SubscriptionResponseDto CreateSubscriptionResponseDto(AppSubscription appSubscription)
        {
            if (appSubscription == null)
            {
                throw new ArgumentNullException(nameof(appSubscription));
            }

            return new SubscriptionResponseDto()
            {
                CancelationDate = appSubscription.CancelationDate,
                NextBillingDate = appSubscription.NextBillingDate,
                Status = appSubscription.Status,
            };
        }

        public async Task<SubscriptionResponseDto> CancelAtEndOfCycle(SubscriptionRequestDto subscriptionRequestDto)
        {
            if(subscriptionRequestDto == null || string.IsNullOrEmpty(subscriptionRequestDto.CustomerId)) throw new ArgumentNullException(nameof(subscriptionRequestDto));

            AppSubscription? subscription = await _subscriptionRepository.Find(s => s.UserId.ToString() == subscriptionRequestDto.UserId);

            if (subscription == null) throw new Exception("Not subscribed!");

            SubscriptionUpdateOptions updateOptions = new SubscriptionUpdateOptions()
            {
                CancelAtPeriodEnd = true,
            };
                        
            var subscriptionService = new SubscriptionService();
            Subscription stripeSubscription;
            try
            {
                stripeSubscription = subscriptionService.Update(subscription.SubscriptionId, updateOptions);
            }catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            };

            subscription.CancelationDate = stripeSubscription.CanceledAt;
            subscription.NextBillingDate = null;
            subscription.Status = "final month";
            
            
            return this.CreateSubscriptionResponseDto(subscription);

        }
        public async Task<SubscriptionResponseDto> ResumeSubscription(SubscriptionRequestDto subscriptionRequestDto)
        {
            if(subscriptionRequestDto == null || string.IsNullOrEmpty(subscriptionRequestDto.CustomerId)) throw new ArgumentNullException(nameof(subscriptionRequestDto));
            
            AppSubscription? subscription = await _subscriptionRepository.Find(s => s.UserId.ToString() == subscriptionRequestDto.UserId);

            if (subscription == null) throw new Exception("Not subscribed!");

            if (subscription.Status == "active") throw new Exception("subscription not in cancelled status");

            if(subscription.Status == "final month")
            {
                var updateOptions = new SubscriptionUpdateOptions()
                {
                    CancelAtPeriodEnd = false,
                };

                var subscriptionService = new SubscriptionService();
                Subscription stripeSubscription;
                try
                {
                    stripeSubscription = subscriptionService.Update(subscription.SubscriptionId, updateOptions);
                }catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                subscription.Status  = "active";
                subscription.NextBillingDate = stripeSubscription.CurrentPeriodEnd;
                subscription.CancelationDate = null;


                return this.CreateSubscriptionResponseDto(subscription);
            }
            if (subscription.Status == "cancelled")
            {
                return new SubscriptionResponseDto()
                {
                    Redirect = true,
                    RedirectUrl = "/checkout-subscription",
                    
                };
            }
            throw new Exception("Not Subscribed");
        }

        public async Task<SubscriptionResponseDto> AddNewPaymentMethod(NewPaymentMethodDto newPaymentMethodDto)
        {
            if(newPaymentMethodDto == null) throw new ArgumentNullException(nameof(newPaymentMethodDto));

            AppSubscription? appSubscription = await _subscriptionRepository.Find(s => s.UserId.ToString() == newPaymentMethodDto.UserId);

            if (appSubscription == null) throw new Exception("No active subscription");

            try
            {
                var options = new PaymentMethodCreateOptions
                {
                    Customer = appSubscription.CustomerId,
                
                    Type = "card",
                    Card = new PaymentMethodCardOptions
                    {
                        Number = newPaymentMethodDto.Number,
                        ExpMonth = newPaymentMethodDto.ExpMonth,
                        ExpYear = newPaymentMethodDto.ExpYear,
                        Cvc = newPaymentMethodDto.Cvc,
                    },
                };
                var paymentService = new PaymentMethodService();
                PaymentMethod paymentMethod = paymentService.Create(options);

                var updateOptions = new CustomerUpdateOptions()
                {
                    InvoiceSettings =
                    {
                        DefaultPaymentMethod = paymentMethod.Id,
                    }
                };

                var customerService = new CustomerService();

                customerService.Update(appSubscription.CustomerId, updateOptions);

                return this.CreateSubscriptionResponseDto(appSubscription);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<SubscriptionResponseDto?> GetSubscriptionDetails(SubscriptionRequestDto subscriptionRequestDto)
        {
            if (subscriptionRequestDto == null)
                throw new Exception(nameof(subscriptionRequestDto));

            AppSubscription? appSubscription = await _subscriptionRepository.Find(s => s.UserId.ToString() == subscriptionRequestDto.UserId);

            if (appSubscription == null) return null;

            return this.CreateSubscriptionResponseDto(appSubscription);
        }


        public async Task<List<PaymentMethodResponseDto>> GetAllPaymentMethods(SubscriptionRequestDto subscriptionRequestDto)
        {
            if(subscriptionRequestDto == null) throw new Exception(nameof(subscriptionRequestDto));

            AppSubscription? appSubscription = await _subscriptionRepository.Find(s => s.UserId.ToString() == subscriptionRequestDto.UserId);
            if(appSubscription == null) return new List<PaymentMethodResponseDto>();


            var options = new CustomerListPaymentMethodsOptions { Limit = 3 };
            var service = new CustomerService();
            StripeList<PaymentMethod> paymentMethods = service.ListPaymentMethods(appSubscription.CustomerId, options);

            List<PaymentMethodResponseDto> responseList = new List<PaymentMethodResponseDto>(); 

            foreach(var paymentMethod in paymentMethods)
            {
                PaymentMethodResponseDto paymentMethodResponse = new PaymentMethodResponseDto();

                paymentMethodResponse.PaymentMethodId = paymentMethod.Id;
                paymentMethodResponse.Last4 = paymentMethod.Card.Last4;
                paymentMethodResponse.ExtMonth = paymentMethod.Card.ExpMonth;
                paymentMethodResponse.ExtYear = paymentMethod.Card.ExpYear;
                paymentMethodResponse.Brand = paymentMethod.Card.Brand;



                responseList.Add(paymentMethodResponse);
            }

            return responseList;

        }

        public async Task<SubscriptionResponseDto> CreateSubscription(SubscriptionRequestDto subscriptionRequestDto)
        {

            //HERE CHECK IF THERE IS ALREADY AN ENTRY IN THE TABLE TO AVOID A CONFILCT BETWEEN WEBHOOK AND "return-payment"

            ApplicationUser? user = await _identityService.GetByUserName(subscriptionRequestDto.UserName ?? "");

            if (user == null)
            {
                throw new Exception("User name not identified");
            }

            Subscription subscription = await _stripeService.GetCustomerSubscription(subscriptionRequestDto.CustomerId ?? "");

            if (subscription == null) throw new Exception("subscription cant be found");

            AppSubscription appSubscription = new AppSubscription()
            {
                AppSubscriptionId = Guid.NewGuid(),
                UserId = user.Id,
                Status = "active",
                UserName = user.UserName,
                UserEmail = user.Email,
                CancelationDate = null,
                CustomerId = subscriptionRequestDto.CustomerId,
                NextBillingDate = subscription.CurrentPeriodEnd, 
                SubscriptionId = subscriptionRequestDto.SubscriptionId,
            };

            await _subscriptionRepository.Create(appSubscription);

            SubscriptionResponseDto subscriptionResponseDto = new SubscriptionResponseDto()
            {
                NextBillingDate = appSubscription.NextBillingDate,
                CancelationDate = appSubscription.CancelationDate,
                Status = appSubscription.Status
            };

            return subscriptionResponseDto;
        }

        public Task UpdateSuscription(SubscriptionRequestDto subscriptionRequestDto)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsSubscriptionActive(SubscriptionRequestDto subscriptionRequestDto)
        {
            //check local subscription entity
            AppSubscription? appSubscription = await _subscriptionRepository.Find(sub => sub.User != null && sub.User.Id.ToString() == subscriptionRequestDto.UserId);

            if (appSubscription == null || appSubscription.CustomerId == null)
            {
                return false;
            }

            //if we have it as active, we search stripe directly
            if(appSubscription.Status == "active")
            {
                Subscription stripeSub = await _stripeService.GetCustomerSubscription(appSubscription.CustomerId);

                if(stripeSub == null)
                {
                    return false;
                }

                if(stripeSub.Status == "active")
                {
                    return true;
                }
            }

            //for testing because we are not adding to the DB right now
            Subscription stripeSub2 = await _stripeService.GetCustomerSubscription(appSubscription.CustomerId);

            if (stripeSub2 == null)
            {
                return false;
            }

            if (stripeSub2.Status == "active")
            {
                return true;
            }

            return false;

        }
        public Task<SubscriptionResponseDto> SuccessfulPayment(SubscriptionRequestDto subscriptionRequestDto)
        {



            throw new NotImplementedException();        
        }


    }
}

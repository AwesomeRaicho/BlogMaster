using BlogMaster.Core.Contracts;
using BlogMaster.Core.DTO;
using BlogMaster.Core.Models;
using BlogMaster.Core.Models.Identity;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<SubscriptionResponseDto> CreateSubscription(SubscriptionRequestDto subscriptionRequestDto)
        {
            var user = await _identityService.GetEntityById(Guid.Parse(subscriptionRequestDto.UserId ?? ""));

            if (user == null)
            {
                throw new InvalidOperationException("User does not exist, must be signed up to subscribe");
            }

            AppSubscription? appSubscription = await _subscriptionRepository.Find(s => s.UserId == subscriptionRequestDto.UserId);


            if (appSubscription == null)
            {
                appSubscription = new AppSubscription()
                {
                    UserId = subscriptionRequestDto.UserId,
                    Status = "Active",
                    UserName = subscriptionRequestDto.UserName,
                    UserEmail = subscriptionRequestDto.UserEmail,
                    CreatedDate = DateTime.UtcNow,
                    CancelationDate = null,
                    CustomerId = subscriptionRequestDto.CustomerId,
                    EndDate = subscriptionRequestDto.EndDate,
                    StartDate = subscriptionRequestDto.StartDate,
                    NextBillingDate = subscriptionRequestDto.NextBillingDate,
                    SubscriptionId = subscriptionRequestDto.SubscriptionId,
                };
            }
            else
            {
                appSubscription.UserId = subscriptionRequestDto.UserId;
                appSubscription.Status = "Active";
                appSubscription.UserName = subscriptionRequestDto.UserName;
                appSubscription.UserEmail = subscriptionRequestDto.UserEmail;
                appSubscription.CancelationDate = null;
                appSubscription.CustomerId = subscriptionRequestDto.CustomerId;
                appSubscription.EndDate = subscriptionRequestDto.EndDate;
                appSubscription.StartDate = subscriptionRequestDto.StartDate;
                appSubscription.NextBillingDate = subscriptionRequestDto.NextBillingDate;
                appSubscription.SubscriptionId = subscriptionRequestDto.SubscriptionId;

            }


 




            SubscriptionResponseDto subscriptionResponseDto = new SubscriptionResponseDto()
            {
                NextBillingDate = appSubscription.NextBillingDate,
                CancelationDate = appSubscription.CancelationDate,
                CreatedDate = appSubscription.CreatedDate,
                EndDate = appSubscription.EndDate,
                StartDate = appSubscription.StartDate,
                Status = appSubscription.Status
                
            };


            return subscriptionResponseDto;
        }
        


    }
}

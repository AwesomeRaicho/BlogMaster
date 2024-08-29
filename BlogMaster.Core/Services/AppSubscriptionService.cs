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
            var user = await _identityService.GetEntityById(subscriptionRequestDto.UserId);

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
                    CreatedDate = DateTime.UtcNow,
                    Status = "Pending",
                    UserName = subscriptionRequestDto.UserName,
                    UserEmail = subscriptionRequestDto.UserEmail,
                };
            }

            Customer customer = _stripeService.CreateStripeCustomer(appSubscription);



            SubscriptionResponseDto subscriptionResponseDto = new SubscriptionResponseDto();


            return subscriptionResponseDto;
        }
        


    }
}

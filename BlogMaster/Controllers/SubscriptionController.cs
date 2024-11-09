using BlogMaster.Core.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System.Security.Claims;


namespace BlogMaster.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly IStripeService _stripeService;
        private readonly IIdentityService _identityService;

        public SubscriptionController(IStripeService stripeService, IIdentityService identityService) 
        {
            _stripeService = stripeService;
            _identityService = identityService;
        }

        
        [Route("/subscription-details")]
        public async Task<IActionResult> SubscriptionDetails()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(User.Identity == null || !User.Identity.IsAuthenticated || userId == null)
            {
                return RedirectToAction("SignIn", "Identity");
            }

            string? stripeCustomerId = await _identityService.GetStripeCustomerId(userId);

            if(stripeCustomerId == null)
            {
                string userName = User.Identity.Name ?? "";
                string userEmail = User.FindFirstValue(ClaimTypes.Email) ?? "";

                if(string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userEmail)) 
                {
                    RedirectToAction("SignIn", "Identity");
                }

                Customer stripeCustomer = _stripeService.CreateStripeCustomer(userName, userEmail);

                 await _identityService.UpdateStripeCustomerId(userId, stripeCustomer.Id);
                stripeCustomerId = stripeCustomer.Id;

            }
                Subscription subscription = await _stripeService.GetCustomerSubscription(stripeCustomerId);

                return View(subscription);
        }

        [Route("/cancel-subscription")]
        public IActionResult CancelSubscription([FromQuery] string subscriptionId)
        {

            if(subscriptionId != null) 
            { 
                _stripeService.CancelSubscription(subscriptionId);
            }

            return RedirectToAction("SubscriptionDetails");
        }

        [Authorize]
        [Route("/resume-subscription")]
        public async Task<IActionResult> ResumeSubscription([FromQuery] string subscriptionId, string customerId)
        {
            
            Subscription subscription = await _stripeService.GetCustomerSubscription(customerId);

            if(subscription != null && subscription.Id == subscriptionId && subscription.CanceledAt != null && subscription.Status == "active") 
            {
                _stripeService.ResumeSubscription(subscriptionId);
            }

            return RedirectToAction("SubscriptionDetails");

        }
    }
}

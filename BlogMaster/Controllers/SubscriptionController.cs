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
        private readonly IConfiguration _configuration;

        public SubscriptionController(IStripeService stripeService, IIdentityService identityService, IConfiguration configuration) 
        {
            _stripeService = stripeService;
            _identityService = identityService;
            _configuration = configuration;
        }

        
        [Route("/subscription-details")]
        public async Task<IActionResult> SubscriptionDetails()
        {
            ViewBag.SignedIn = User.Identity?.IsAuthenticated;
            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];

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
            ViewBag.SignedIn = User.Identity?.IsAuthenticated;
            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];

            if (subscriptionId != null) 
            { 
                _stripeService.CancelSubscription(subscriptionId);
            }

            return RedirectToAction("SubscriptionDetails");
        }

        [Authorize]
        [Route("/resume-subscription")]
        public async Task<IActionResult> ResumeSubscription([FromQuery] string subscriptionId, string customerId)
        {
            ViewBag.SignedIn = User.Identity?.IsAuthenticated;
            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];

            Subscription subscription = await _stripeService.GetCustomerSubscription(customerId);

            if(subscription != null && subscription.Id == subscriptionId && subscription.CanceledAt != null && subscription.Status == "active") 
            {
                _stripeService.ResumeSubscription(subscriptionId);
            }

            return RedirectToAction("SubscriptionDetails");

        }

        [Authorize]
        [Route("/payment-methods")]
        public IActionResult PaymentMethods([FromQuery] string customerId, string method)
        {
            ViewBag.SignedIn = User.Identity?.IsAuthenticated;
            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];
            ViewBag.PayMethod = method;
            StripeList<PaymentMethod> paymethods = _stripeService.StripePaymentMethods(customerId);

            return View(paymethods);
        }

        //deault
        [Authorize]
        [Route("/default-payment-method")]
        public IActionResult DefaultPaymentMethod(string methodId)
        {
            ViewBag.SignedIn = User.Identity?.IsAuthenticated;
            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];

            return View();
        }


        //remove 
        [Authorize]
        [Route("/remove-payment-method")]
        public IActionResult RemovePaymentMethod(string methodId)
        {

            ViewBag.SignedIn = User.Identity?.IsAuthenticated;
            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];

            return View();
        }
    }
}

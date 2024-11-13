using BlogMaster.Core.Contracts;
using BlogMaster.Core.DTO;
using BlogMaster.Core.Models;
using BlogMaster.Infrastructure.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using Stripe.Checkout;
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
                Stripe.Subscription subscription = await _stripeService.GetCustomerSubscription(stripeCustomerId);

                return View(subscription);
        }

        [Authorize]
        [Route("/payment-methods")]
        public IActionResult PaymentMethods([FromQuery] string customerId, string method, string subscriptionId)
        {


            ViewBag.PublishableKey = _stripeService.GetPublishableKey();
            ViewBag.SignedIn = User.Identity?.IsAuthenticated;
            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];
            ViewBag.PayMethod = method;
            ViewBag.SubscriptionId = subscriptionId;
            ViewBag.CustomerId = customerId;
            StripeList<PaymentMethod> paymethods = _stripeService.StripePaymentMethods(customerId);

            return View(paymethods);
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

            return RedirectToAction("SubscriptionDetails", new { subscriptionId =  subscriptionId });
        }

        [Authorize]
        [Route("/resume-subscription")]
        public async Task<IActionResult> ResumeSubscription([FromQuery] string subscriptionId, string customerId)
        {
            ViewBag.SignedIn = User.Identity?.IsAuthenticated;
            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];

            Stripe.Subscription subscription = await _stripeService.GetCustomerSubscription(customerId);

            if(subscription != null && subscription.Id == subscriptionId && subscription.CanceledAt != null && subscription.Status == "active") 
            {
                _stripeService.ResumeSubscription(subscriptionId);
            }

            return RedirectToAction("SubscriptionDetails");

        }



        //deault
        [Authorize]
        [Route("/default-payment-method")]
        public IActionResult DefaultPaymentMethod(string methodId, string customerId, string subscriptionId)
        {

            _stripeService.ChangeDefaultPaymentMethod(customerId, methodId, subscriptionId);

            return RedirectToAction("PaymentMethods", new { subscriptionId, customerId, method = methodId });

        }


        //remove 
        [Authorize]
        [Route("/remove-payment-method")]
        public IActionResult RemovePaymentMethod(string methodId, string customerId, string defaultMethod, string subscriptionId )
        {

            _stripeService.RemovePaymentMethod(methodId);

            return RedirectToAction("PaymentMethods", new { customerId , method = defaultMethod, subscriptionId   });
        }

        [Authorize]
        [Route("/create-payment-method")]
        public IActionResult CreatePaymentMethod([FromQuery] string customerId)
        {

            string? email = User.FindFirstValue(ClaimTypes.Email);

            SetupIntent intent = _stripeService.StartSessionForEmbededIntent(customerId, email);
            Session session = _stripeService.StartSessionForEmbededSession(customerId, email);

            
            return Json(new { clientSecret = session.ClientSecret });
        }

    }
}

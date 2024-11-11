using BlogMaster.Core.Contracts;
using BlogMaster.Core.DTO;
using BlogMaster.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace BlogMaster.Controllers
{
    public class PaymentController : Controller
    {

        private readonly IStripeService _stripeService;
        private readonly IAppSubscriptionService _appSubscriptionService;
        private readonly IIdentityService _identityService;

        public PaymentController(IStripeService stripeService, IAppSubscriptionService appSubscriptionService, IIdentityService identityService)
        {
            _stripeService = stripeService;
            _appSubscriptionService = appSubscriptionService;
            _identityService = identityService;
        }

        //SUBSCRIPTION
        //[Authorize]
        [HttpGet("/checkout-subscription")]
        public async Task<ActionResult> PayFormSubscription(GetFormRequestDto getFormRequestDto)
        {
            string? userName = User.Identity?.Name;
            string? userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //check if there is an active subscription
            SubscriptionRequestDto requestDto = new SubscriptionRequestDto()
            {
                UserName = userName,
                UserEmail = userEmail,
                UserId = userId
            };

            ViewBag.PublishableKey = _stripeService.GetPublishableKey();

            string? CustomerId = await _identityService.GetStripeCustomerId(userId ?? "");

            if(CustomerId != null)
            {
                string? isSubscripbed = await _stripeService.SubscriptionStatus(CustomerId);

                if(!string.IsNullOrEmpty(isSubscripbed) && isSubscripbed == "Active") 
                { 
                    return RedirectToAction("SubscriptionDetails", "Subscription");
                }   
            };

            return View();
        }
    
        //[Authorize]
        [HttpPost("/checkout-session-subscription")]
        public async Task<IActionResult> CheckoutSessionSubscription(GetFormRequestDto getFormRequestDto)
        {
            getFormRequestDto.Username = User.Identity?.Name;
            getFormRequestDto.UserEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            SessionReturnDto session = await _stripeService.StartSessionForEmbededFormSubscription(getFormRequestDto);

            //check if redirection is null, if not, redirect to url given

            return Json(new { clientSecret = session.StripeSession?.ClientSecret });
        }

        //DONTATION

        [HttpGet("/checkout-donation")]
        public ActionResult PayFormDonation(GetFormRequestDto getFormRequestDto)
        {
            getFormRequestDto.Amount = 100;

            ViewBag.PublishableKey = _stripeService.GetPublishableKey();

            return View(getFormRequestDto);
        }

        [HttpPost("/create-checkout-session-donation")]
        public async Task<IActionResult> CheckoutSessionDonation(GetFormRequestDto getFormRequestDto)
        {
            Session session =  await _stripeService.StartSessionForEmbededFormDonation(getFormRequestDto);

            return Json(new { clientSecret = session.ClientSecret });
        }

        [HttpGet("/payment-return")]
        public async Task<IActionResult> PaymentReturn([FromQuery] string session_id)
        {
            if (session_id == null)
            {
                throw new ArgumentNullException(nameof(session_id)); 
            }


            // Returned data needs to be saved for billing cycle, payment history, 

            var sessionService = new SessionService();
            Session session = sessionService.Get(session_id);

            PaymentHistory entry = new PaymentHistory()
            {
                Amount = session.AmountSubtotal,
                PaymentId = Guid.NewGuid(),
                PaymentStatus = session.PaymentStatus,
                PaymentDate = DateTime.UtcNow,
                CustomerId = session.CustomerId,
                SubscriptionId = session.SubscriptionId,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? null,
            };
            
            if(session.Mode == "subscription")
            {
                //get subscription
                var stripeServiceSubscription = new SubscriptionService();
                Subscription stripeSubscriptionResponse = stripeServiceSubscription.Get(session.SubscriptionId);

                SubscriptionRequestDto subscriptionRequestDto = new SubscriptionRequestDto()
                {
                    UserEmail = session.CustomerEmail,
                    UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    UserName = User.Identity?.Name,
                    SubscriptionId= session.SubscriptionId,
                    CustomerId = session.CustomerId,
                    StartDate = stripeSubscriptionResponse.StartDate,
                    EndDate = stripeSubscriptionResponse.CurrentPeriodEnd,
                    NextBillingDate = stripeSubscriptionResponse.CurrentPeriodEnd
                };

                var subscriptionResponse = await _appSubscriptionService.CreateSubscription(subscriptionRequestDto);

                //Return a subscription screen
                return View(subscriptionResponse);
            }

            //return a regular donation
            return View();
        }

    }
}

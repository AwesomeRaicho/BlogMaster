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

        public PaymentController(IStripeService stripeService, IAppSubscriptionService appSubscriptionService)
        {
            _stripeService = stripeService;
            _appSubscriptionService = appSubscriptionService;
        }


        //SUBSCRIPTION
        //[Authorize]
        [HttpGet("/checkout-subscription")]
        public async Task<ActionResult> PayFormSubscription(GetFormRequestDto getFormRequestDto)
        {
            //check if there is an active subscription
            SubscriptionRequestDto requestDto = new SubscriptionRequestDto()
            {
                UserName = User.Identity?.Name,
                UserEmail = User.FindFirst(ClaimTypes.Email)?.Value,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            };

            requestDto.UserName = "Raicho";
            requestDto.UserEmail = "testing@testing.com";



            ViewBag.PublishableKey = _stripeService.GetPublishableKey();

            bool isSubscripbed = await _appSubscriptionService.IsSubscriptionActive(requestDto);

            if(isSubscripbed)
            {
                return RedirectToAction("Subscription", "SubscriptionDetails");
            }


            return View();
        }
    
        //[Authorize]
        [HttpPost("/create-checkout-session-subscription")]
        public async Task<IActionResult> CheckoutSessionSubscription(GetFormRequestDto getFormRequestDto)
        {

            getFormRequestDto.Username = User.Identity?.Name;
            getFormRequestDto.UserEmail = User.FindFirst(ClaimTypes.Email)?.Value;


            //these new two lines are replacing the previous 2 for testing purposes (to be eliminated)
            getFormRequestDto.Username = "Raicho";
            getFormRequestDto.UserEmail = "testing@testing.com";

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
        public IActionResult PaymentReturn([FromQuery] string session_id)
        {
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
                var service = new SubscriptionService();
                Subscription subscriptionResponse = service.Get(session.SubscriptionId);


                SubscriptionRequestDto subscriptionRequestDto = new SubscriptionRequestDto()
                {
                    UserEmail = session.CustomerEmail,
                    UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    UserName = User.Identity?.Name,
                    SubscriptionId= session.SubscriptionId,
                    CustomerId = session.CustomerId,
                    StartDate = subscriptionResponse.StartDate,
                    EndDate = subscriptionResponse.CurrentPeriodEnd,
                    NextBillingDate = subscriptionResponse.CurrentPeriodEnd
                };

                //Return a subscription screen
                return View(subscriptionRequestDto);

            }

            //return a regular donation
            return View();
        }





    }
}

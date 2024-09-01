using BlogMaster.Core.Contracts;
using BlogMaster.Core.DTO;
using BlogMaster.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace BlogMaster.Controllers
{
    public class PaymentController : Controller
    {

        private readonly IStripeService _stripeService;

        public PaymentController(IStripeService stripeService)
        {
            _stripeService = stripeService;
        }


        //SUBSCRIPTION
        //[Authorize]
        [HttpGet("/checkout-subscription")]
        public ActionResult PayFormSubscription(GetFormRequestDto getFormRequestDto)
        {

            ViewBag.PublishableKey = _stripeService.GetPublishableKey();
            return View();
        }


        //[Authorize]
        [HttpPost("/create-checkout-session-subscription")]
        public async Task<IActionResult> CheckoutSessionSubscription(GetFormRequestDto getFormRequestDto)
        {

            getFormRequestDto.Username = User.Identity?.Name;
            getFormRequestDto.UserEmail = User.FindFirst(ClaimTypes.Email)?.Value;


            //these new two lines are replacing the previous 2 for testing purposes (to be eliminated)
            getFormRequestDto.Username = "new person maybe";
            getFormRequestDto.UserEmail = "TestExample@testexample.com";

            Session session = await _stripeService.StartSessionForEmbededFormSubscription(getFormRequestDto);

            return Json(new { clientSecret = session.ClientSecret });
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
                SubscriptionRequestDto subscriptionRequestDto = new SubscriptionRequestDto()
                {
                    UserEmail = session.CustomerEmail,
                    UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    UserName = User.Identity?.Name,
                    SubscriptionId= session.SubscriptionId,
                    CustomerId = session.CustomerId,
                    StartDate = session.Subscription.StartDate,
                    EndDate = session.Subscription.CurrentPeriodEnd,
                    NextBillingDate = session.Subscription.CurrentPeriodEnd
                };

                //Return a subscription screen
                return View(subscriptionRequestDto);

            }



            //return a regular donation
            return View();
        }

    }
}

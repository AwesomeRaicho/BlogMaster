using BlogMaster.Core.Contracts;
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
        [HttpPost("/create-checkout-session-Subscription")]
        public async Task<IActionResult> CheckoutSessionSubscription(GetFormRequestDto getFormRequestDto)
        {



            Session session = await _stripeService.StartSessionForEmbededFormDonation(getFormRequestDto);

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
            //Returned data needs to be saved for billing cycle, payment history, 

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
                AppSubscription appSubscription = new AppSubscription()
                {

                };
            }


            return View();
        }

    }
}

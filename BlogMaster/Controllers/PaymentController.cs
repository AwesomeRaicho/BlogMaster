using BlogMaster.Core.Contracts;
using BlogMaster.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace BlogMaster.Controllers
{
    public class PaymentController : Controller
    {

        private readonly IStripeService _stripeService;

        public PaymentController(IStripeService stripeService)
        {
            _stripeService = stripeService;
        }


        [HttpGet("/checkout")]
        public ActionResult PayForm(GetFormRequestDto getFormRequestDto)
        {

            ViewBag.PublishableKey = _stripeService.GetPublishableKey();
            return View();
        }



        [HttpPost("/create-checkout-session")]
        public async Task<IActionResult> CheckoutSession(GetFormRequestDto getFormRequestDto)
        {



            Session session =  _stripeService.StartSessionForEmbededForm(getFormRequestDto);

            return Json(new { clientSecret = session.ClientSecret });
        }


  

    }
}

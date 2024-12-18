﻿using BlogMaster.Core.Contracts;
using BlogMaster.Core.DTO;
using BlogMaster.Core.Models;
using BlogMaster.Infrastructure.Migrations;
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
        private readonly IIdentityService _identityService;
        private readonly IConfiguration _configuration;


        public PaymentController(IStripeService stripeService, IIdentityService identityService, IConfiguration configuration)
        {
            _stripeService = stripeService;
            _identityService = identityService;
            _configuration = configuration;
        }

        //SUBSCRIPTION
        //[Authorize]
        [HttpGet("/checkout-subscription")]
        public async Task<ActionResult> PayFormSubscription(GetFormRequestDto getFormRequestDto)
        {
            ViewBag.SignedIn = User.Identity?.IsAuthenticated;
            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];

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

            if(!string.IsNullOrEmpty(CustomerId))
            {
                string? isSubscripbed = await _stripeService.SubscriptionStatus(CustomerId);

                if(!string.IsNullOrEmpty(isSubscripbed) && isSubscripbed == "active") 
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
            getFormRequestDto.UserName = User.Identity?.Name;
            getFormRequestDto.UserEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            SessionReturnDto session = await _stripeService.StartSessionForEmbededFormSubscription(getFormRequestDto);

            //check if redirection is null, if not, redirect to url given

            return Json(new { clientSecret = session.StripeSession?.ClientSecret });
        }

        //DONTATION

        [HttpPost("/checkout-donation")]
        public ActionResult PayFormDonation(GetFormRequestDto getFormRequestDto)
        {
            if(getFormRequestDto.Amount < 20)
            {
                return RedirectToAction("Index", "Site");
            }

            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];

            ViewBag.SignedIn = User.Identity?.IsAuthenticated;

            if(ViewBag.SignedIn)
            {
                ViewBag.UserName = User.Identity?.Name;
                ViewBag.UserEmail = User.FindFirstValue(ClaimTypes.Email);
            }

            ViewBag.PublishableKey = _stripeService.GetPublishableKey();

            return View(getFormRequestDto);
        }

        [HttpPost("/create-checkout-session-donation")]
        public async Task<IActionResult> CheckoutSessionDonation(GetFormRequestDto getFormRequestDto)
        {
            string? email = User.FindFirstValue(ClaimTypes.Email);

            if(!string.IsNullOrEmpty(email))
            {
                getFormRequestDto.UserEmail = email;
            }
            Session session =  await _stripeService.StartSessionForEmbededFormDonation(getFormRequestDto);

            return Json(new { clientSecret = session.ClientSecret });
        }

        [HttpGet("/payment-return")]
        public async Task<IActionResult> PaymentReturn([FromQuery] string session_id)
        {

            if(session_id == null)
            {
                return RedirectToAction("SubscriptionDetails", "Subscription");

            }

            // Returned data needs to be saved for billing cycle, payment history, 
            

            var sessionService = new SessionService();
            Session session = sessionService.Get(session_id);

            if (session.Mode == "subscription")
            {
                return RedirectToAction("SubscriptionDetails", "Subscription");

            }

            if(session.Mode == "payment")
            {
                return RedirectToAction("DonationThanks", "Site");
            }



            if (session.SetupIntentId != null)
            {
                var service = new SetupIntentService();
                var SetupIntent = service.Get(session.SetupIntentId);

                var paymentMethodAttachOptions = new PaymentMethodAttachOptions { Customer = SetupIntent.CustomerId };
                Stripe.Subscription subscription = await _stripeService.GetCustomerSubscription(SetupIntent.CustomerId);

                var paymentMethodService = new PaymentMethodService();
                paymentMethodService.Attach(SetupIntent.PaymentMethodId, paymentMethodAttachOptions);
            //string customerId, string method, string subscriptionId
                return RedirectToAction("PaymentMethods", "Subscription", new { customerId = SetupIntent.CustomerId, method = subscription.DefaultPaymentMethodId, subscriptionId = subscription.Id });

            }



            return RedirectToAction("Index", "Site");

        }

    }
}

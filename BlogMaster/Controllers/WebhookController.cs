using BlogMaster.Core.Contracts;
using BlogMaster.Core.DTO;
using BlogMaster.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using System.IO;
using System.Threading.Tasks;




[ApiController]
public class WebhookController : Controller
{

    private string? endpointSecret;
    private IAppSubscriptionService _subscriptionService;



    public WebhookController(IConfiguration configuration, IAppSubscriptionService appSubscriptionService)
    {
        endpointSecret = configuration["Stripe:SercretEndpoint"];
        _subscriptionService = appSubscriptionService;
    }

    [Route("/webhook")]
    [HttpPost]
    public async Task<IActionResult> Index()
    {
        string json;

        using (var reader = new StreamReader(HttpContext.Request.Body))
        {
            json = await reader.ReadToEndAsync();
        }



        try
        {
            var stripeEvent = EventUtility.ParseEvent(json);
            var signatureHeader = Request.Headers["Stripe-Signature"];

            stripeEvent = EventUtility.ConstructEvent(json,
                    signatureHeader, endpointSecret);

            if (stripeEvent.Type == Events.InvoicePaid)
            {




                var invoice = stripeEvent.Data.Object as Stripe.Invoice;

                if (invoice != null)
                {
                    SubscriptionRequestDto subRequest = new SubscriptionRequestDto()
                    {
                        CustomerId = invoice.CustomerId,
                        SubscriptionId = invoice.SubscriptionId,
                        UserName = invoice.CustomerName,
                        UserEmail = invoice.CustomerEmail,

                    };

                     await _subscriptionService.CreateSubscription(subRequest);


                }
            }
            else if (stripeEvent.Type == Events.InvoicePaymentFailed)
            {
                var invoice = stripeEvent.Data.Object as Stripe.Invoice;






                // Then define and call a method to handle the successful attachment of a PaymentMethod.
                // handlePaymentMethodAttached(paymentMethod);
            }
            else
            {
                Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
            }
            return Ok();
        }
        catch (StripeException e)
        {
            Console.WriteLine("Error: {0}", e.Message);
            return BadRequest();
        }
        catch (Exception e)
        {
            return StatusCode(500);
        }
    }
}

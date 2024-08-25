using BlogMaster.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace BlogMaster.Controllers
{
    public class TestingController : Controller
    {
        private readonly IEmailService _emailService;

        public TestingController(IEmailService emailService)
        {
            _emailService = emailService;
        }





        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/testemail")]
        public IActionResult TestEmail()
        {
            _emailService.TestSendEmail("", "Raicho", "852963741");


            return View();
        }


        








    }
}

using BlogMaster.Core.Contracts;
using BlogMaster.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace BlogMaster.Controllers
{
    public class TestingController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly IAppSubscriptionService _appSubscriptionService;

        public TestingController(IEmailService emailService, IAppSubscriptionService appSubscriptionService)
        {
            _emailService = emailService;
            _appSubscriptionService = appSubscriptionService;
        }





        [Route("/test")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/testemail")]
        public IActionResult TestEmail()
        {



            return View();
        }

        [Route("/subscribe-user-test")]
        public IActionResult TestSubscribeUser()
        {
            //_appSubscriptionService.SubscribeUser(new Core.DTO.IdentityResponseDto());


            return View();
        }



        [HttpGet]
        [Route("/add-image-test")]
        public IActionResult AddImageTest()
        {

            return View();
        }

        [HttpPost]
        [Route("/add-image-test")]
        public async Task<IActionResult> AddImageTest(IFormFile image)
        {

            return View();
        }






    }
}

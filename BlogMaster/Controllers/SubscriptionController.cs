using Microsoft.AspNetCore.Mvc;

namespace BlogMaster.Controllers
{
    public class SubscriptionController : Controller
    {
        [Route("/subscription-details")]
        public IActionResult SubscriptionDetails()
        {


            return View();
        }
    }
}

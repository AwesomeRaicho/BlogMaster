using Microsoft.AspNetCore.Mvc;

namespace BlogMaster.Controllers
{
    public class SiteController : Controller
    {

        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace BlogMaster.Controllers
{
    public class SiteController : Controller
    {

        [Route("/")]
        public IActionResult Index()
        {
            ViewBag.Title = "Blog Master";
            ViewBag.SignedIn = User.Identity?.IsAuthenticated;


            return View();
        }

        [Route("/blogs")]
        public IActionResult Blogs()
        {
            ViewBag.Title = "Blog Master";
            ViewBag.SignedIn = User.Identity?.IsAuthenticated;



            return View();
        }


    }
}

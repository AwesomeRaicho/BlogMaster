using BlogMaster.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BlogMaster.Controllers
{
    public class IdentityController : Controller
    {

        [HttpGet]
        [Route("/register")]
        public IActionResult Registration()
        {




            return View();
        }


        [HttpPost]
        [Route("/register")]
        public async Task<IActionResult> Registration(IdentityRegistrationDto registrationDto)
        {




            return View();
        }
    }
}

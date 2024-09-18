using BlogMaster.Core.DTO;
using Microsoft.AspNetCore.Mvc;
using BlogMaster.Controllers.Helpers;
using BlogMaster.Core.Contracts;
using Microsoft.AspNetCore.Identity;

namespace BlogMaster.Controllers
{
    public class IdentityController : Controller
    {

        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }



        [HttpGet]
        [Route("/registration")]
        public IActionResult Registration()
        {


            return View();
        }


        [HttpPost]
        [Route("/registration")]
        public async Task<IActionResult> Registration(IdentityRegistrationDto registrationDto)
        {
            if (!ModelState.IsValid || registrationDto.Password != registrationDto.ConfirmPassword)
            {
                Dictionary<string, List<string>> errors = ControllerHelper.GetErrors(ModelState);
                if (registrationDto.Password != registrationDto.ConfirmPassword)
                {
                    if (!errors.ContainsKey("ConfirmPassword"))
                    {
                        errors["ConfirmPassword"] = new List<string>();
                    }

                    errors["ConfirmPassword"].Add("Password does not match");
                }

                return View(errors);
            }

            //here I would need to work on the confirmation email
            var result = await _identityService.RegisterUserAsync(registrationDto);

            if (result.Succeeded)
            {
                return RedirectToAction("RegistrationConfirmation");

            }
            throw new Exception("something went wrong!");
        }


        [HttpGet]
        [Route("/identity/confirmemail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token) || !Guid.TryParse(userId, out var userGuid))
            {
                return RedirectToAction("Index", "Home");
            }

            var requestDto = new IdentityRequestDto
            {
                UserId = userGuid,
            };

            var user = await _identityService.GetUserByIdAsync(requestDto);
            if (user == null)
            {
                // User not found, redirect to home page
                return RedirectToAction("Index", "Home");
            }

            var result = await _identityService.ConfirmEmailAsync(userId, token);

            if (result.Succeeded)
            {
                return RedirectToAction("ConfirmEmailSuccess");
            }

            return RedirectToAction("Index", "Home");
        }


        [Route("/RegistrationConfirmation")]
        public IActionResult RegistrationConfirmation()
        {
            return View();
        }

        [Route("confirmemailsuccess")]
        public IActionResult ConfirmEmailSuccess()
        {
            return View();
        }


        [Route("/signin")]
        public IActionResult SignIn() 
        {
            return View();
        }
    }
}

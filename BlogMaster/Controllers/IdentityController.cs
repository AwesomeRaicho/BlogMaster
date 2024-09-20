using BlogMaster.Core.DTO;
using Microsoft.AspNetCore.Mvc;
using BlogMaster.Controllers.Helpers;
using BlogMaster.Core.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

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
            else
            {
                
                List<string> InvalidData = new List<string>();

                foreach(var error in result.Errors)
                {
                    InvalidData.Add(error.Description);
                }

                ViewBag.InvalidData = InvalidData.Count > 0 ? InvalidData : null;

                return View();
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



        // SIGNING IN
        [HttpGet]
        [Route("/signin")]
        public IActionResult SignIn() 
        {



            return View();
        }

        [HttpPost]
        [Route("/signin")]
        public async Task<IActionResult> SignIn(IdentityRequestDto requestDto)
        {
            List<string> errors = new List<string>();
            
            if ( requestDto == null ||string.IsNullOrEmpty(requestDto.UserName) || string.IsNullOrEmpty(requestDto.Password))
            {

                errors.Add("Both user and password need to be provided.");
                               
                return View(errors);
            }

            SignInResponseDto response =  await _identityService.SignIn(requestDto);

            if (response.IsSeccess == false)
            {
                errors.Add($"{response.ErrorMessage}");
                return View(errors);

            }


            return RedirectToAction("Index", "Site");

        }

    }
}

using BlogMaster.Core.DTO;
using Microsoft.AspNetCore.Mvc;
using BlogMaster.Controllers.Helpers;
using BlogMaster.Core.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BlogMaster.Controllers
{
    public class IdentityController : Controller
    {

        private readonly IIdentityService _identityService;
        private readonly IConfiguration _configuration;

        public IdentityController(IIdentityService identityService, IConfiguration configuration)
        {
            _identityService = identityService;
            _configuration = configuration;
        }



        [HttpGet]
        [Route("/registration")]
        public IActionResult Registration()
        {
            ViewBag.SignedIn = User.Identity?.IsAuthenticated;
            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];

            return View();
        }


        [HttpPost]
        [Route("/registration")]
        public async Task<IActionResult> Registration(IdentityRegistrationDto registrationDto)
        {

            ViewBag.SignedIn = User.Identity?.IsAuthenticated;
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

        [Route("/confirmemailsuccess")]
        public IActionResult ConfirmEmailSuccess()
        {
            return View();
        }



        // SIGNING IN
        [HttpGet]
        [Route("/signin")]
        public IActionResult SignIn(List<string> errors) 
        {

            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];

            return View(errors);
        }

        [HttpPost]
        [Route("/signinsubmit")]
        public async Task<IActionResult> SignInSubmit(IdentityRequestDto requestDto)
        {
            ViewBag.SignedIn = User.Identity?.IsAuthenticated;
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
                return RedirectToAction("SignIn", new { errors });
            }


            return RedirectToAction("Index", "Site");

        }


        [HttpGet]
        [Route("/logout")]
        public async Task<IActionResult> LogOut()
        {
            bool? IsAuthenticated = User.Identity?.IsAuthenticated;
            if(IsAuthenticated == true) 
            {
                await _identityService.LogOut();
            }
            Response.Cookies.Delete("subed");
            return RedirectToAction("Index", "Site");

        }


        [Authorize]
        [HttpGet]
        [Route("/password-change")]
        public IActionResult PasswordChange(List<string> errors)
        {
            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];

            ViewBag.Title = "Password Change";
            ViewBag.SignedIn = User.Identity?.IsAuthenticated;

            if(errors == null)
            {
                errors = new List<string>();
            }

            return View(errors);
        }

        [Authorize]
        [HttpPost]
        [Route("/password-change")]
        public async Task<IActionResult> PasswordChange(string Password)
        {
            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];

            ViewBag.Title = "Password Change";
            ViewBag.SignedIn = User.Identity?.IsAuthenticated;
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("SignIn", "Identity");
            }

            bool IsCorrectPassword = await _identityService.IsCorrectPassword(userId, Password);

            if (IsCorrectPassword)
            {
                //try to send email confirmation, if successful, sent to passwordemail view
                if(await _identityService.SendChangePasswordEmailConfirmation(userId))
                {
                    return RedirectToAction("PasswordEmail");
                };
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [Route("/password-email-sent")]
        public async Task<IActionResult> PasswordEmail()
        {
            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];

            await _identityService.LogOut();

            ViewBag.Title = "Sent password email";

            ViewBag.SignedIn = User?.Identity?.IsAuthenticated;

            return View();
        }

        [HttpGet]
        [Route("/password-reset-confirmation")]
        public IActionResult PasswordEmailConfirmation(List<string> errors, string UserId, string Token)
        {
            if (errors == null)
            {
                errors = new List<string>();
            }

            if (UserId == null)
            {
                errors.Add("No user ID detected.");
            }

            if (Token == null)
            {
                errors.Add("No reset token provided.");
            }

            return View(new
            {
                errors,
                UserId,
                Token
            });
        }


        [HttpPost]
        [Route("/password-reset-confirmation")]
        public async Task<IActionResult> PasswordEmailConfirmation(PasswordResetDto passwordResetDto)
        {
            if (passwordResetDto.NewPassword != passwordResetDto.ConfirmNewPassword)
            {
                return View(new
                {
                    errors = new List<string> { "New Password and New Password Confirmation do not match" },
                    UserId = passwordResetDto.UserId,
                    Token = passwordResetDto.Token
                });
            }

            var result = await _identityService.ChangePassword(passwordResetDto);

            if (result == null)
            {
                return View(new
                {
                    errors = new List<string> { "Missing fields in form" },
                    UserId = passwordResetDto.UserId,
                    Token = passwordResetDto.Token
                });
            }

            if (result.Succeeded)
            {
                return RedirectToAction("PasswordResetSuccessful");
            }
            else
            {
                var invalidData = result.Errors.Select(e => e.Description).ToList();

                return View(new
                {
                    errors = invalidData,
                    UserId = passwordResetDto.UserId,
                    Token = passwordResetDto.Token
                });
            }

            throw new Exception("Something went wrong!");
        }



        [HttpGet]
        [Route("password-reset-successfuly")]
        public IActionResult PasswordResetSuccessful()
        {

            return View();
        }

    }
}

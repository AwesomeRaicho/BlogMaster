using BlogMaster.Controllers.Helpers;
using BlogMaster.Core.Contracts;
using BlogMaster.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlogMaster.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministratorController : Controller
    {
        private readonly IIdentityService _identityService;

        public AdministratorController(IIdentityService identityService) 
        {
            _identityService = identityService;
        }




       [Route("/Administrator")]
        public IActionResult AdministratorIndex()
        {

            return View();
        }


        [HttpGet]
        [Route("/create-account")]
        public IActionResult CreateAccount()
        {
            return View();
        }

        [HttpPost]
        [Route("/create-account")]
        public async Task<IActionResult> CreateAccount(AccountCreationRequestDto accountCreationRequestDto)
        {
            if (!ModelState.IsValid)
            {
                Dictionary<string, List<string>> errors = ControllerHelper.GetErrors(ModelState);
                
                return View(errors);
            }

            var result =  await _identityService.CreateAccount(accountCreationRequestDto);

            if(result.Succeeded)
            {
                return RedirectToAction("AdministratorIndex", "Administrator");
            }
            else
            {
                Dictionary<string, List<string>> creationErrors = new Dictionary<string, List<string>>();

                creationErrors.Add("CreationError", new List<string>());

                foreach(var error in result.Errors)
                {
                    creationErrors["CreationError"].Add(error.Description);
                }
                return View(creationErrors);
            }
        }


        [Route("/writters-editors")]
        public async Task<IActionResult> WrittersEditors()
        {
            List<IdentityResponseDto> users = await _identityService.GetWrittersEditors();



            return View(users);
        }

    }
}

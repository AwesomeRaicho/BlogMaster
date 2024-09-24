using BlogMaster.Core.Contracts;
using BlogMaster.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogMaster.Controllers
{
    [Authorize(Roles = "Administrator,Writter,Editor")]
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;


        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }





        [Authorize(Roles = "Administrator,Writter")]
        [HttpGet]
        [Route("/create-blog")]
        public IActionResult CreateBlog()
        {

            return View();
        }

        [Authorize(Roles = "Administrator,Writter")]
        [HttpPost]
        [Route("/create-blog")]
        public async Task<IActionResult> CreateBlog(BlogPostPutDto blogPost)
        {
            string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null)
            {
                return RedirectToAction("SignIn");
            }
            blogPost.UserId = Guid.Parse(id);

            await _blogService.CreateBlogAsync(blogPost);
            return RedirectToAction("AdministratorIndex", "Administrator");
        }


    }
}

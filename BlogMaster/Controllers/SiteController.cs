using BlogMaster.Core.Contracts;
using BlogMaster.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BlogMaster.Controllers
{
    public class SiteController : Controller
    {

        private readonly IBlogService _blogService;


        public SiteController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [Route("/")]
        public IActionResult Index()
        {
            ViewBag.Title = "Blog Master";
            ViewBag.SignedIn = User.Identity?.IsAuthenticated;
            string? user = User.Identity?.Name;
            if (User.IsInRole("Administrator"))
            {
                return RedirectToAction("AdministratorIndex", "Administrator");
            }
            return View();
        }

        [Route("/blogs")]
        public async Task<IActionResult> Blogs([FromQuery] int pageIndex, string category, List<string> tags)
        {
            if (pageIndex == 0)
            {
                pageIndex = 1;
            }
            if (string.IsNullOrEmpty(category))
            {
                category = "";
            }
            ViewBag.Title = "Blogs";
            ViewBag.SignedIn = User.Identity?.IsAuthenticated;
            ViewBag.PageIndex = pageIndex;
            ViewBag.Category = category;
            ViewBag.Tags = tags.Count != 0 && tags[0] != null ? tags : null;


            var previews = await _blogService.GetAllBlogPreviews(pageIndex, category, tags.Count != 0 && tags[0] != null ? tags : new List<string>());

            return View(previews);
        }


        [Route("/blogs/blogpage/{slug?}")]
        public async Task<IActionResult> BlogPage(string? slug = null)
        { 
            if (string.IsNullOrEmpty(slug))
            {
                return RedirectToAction("Blogs");
            }

            ViewBag.SignedIn = User.Identity?.IsAuthenticated;

            BlogResponseDto? blog = await _blogService.GetBlogBySlug(slug);

            if (blog == null)
            {
                return NotFound(); 
            }

            return View(blog);
        }
    }
}

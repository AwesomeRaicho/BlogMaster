using BlogMaster.Core.Contracts;
using BlogMaster.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogMaster.Controllers
{
    [Authorize(Roles = "Administrator,Writter")]
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;


        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }





        [HttpGet]
        [Route("/create-blog")]
        public IActionResult CreateBlog()
        {

            return View();
        }

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

        [HttpGet]
        [Route("/admin-blog-views")]
        public async Task<IActionResult> AdminBlogViews([FromQuery] int pageIndex, string category, List<string> tags)
        {


            if(pageIndex == 0)
            {
                pageIndex = 1;
            }
            if(string.IsNullOrEmpty(category))
            {
                category = "";
            }
            ViewBag.PageIndex = pageIndex;
            ViewBag.Category = category;
            ViewBag.Tags = tags.Count != 0 && tags[0] != null ? tags: null;


            var previews = await _blogService.GetAllAdminBlogPreviews(pageIndex, category, tags.Count != 0 && tags[0] != null ? tags : new List<string>());

            return View(previews);
        }

        [Route("/categories")]
        public async Task<IActionResult> Categories()
        {
            IEnumerable<CategoryResponseDto> categories = await _blogService.GetAllCategories();
            return View(categories);
        }
             
        [HttpGet]
        [Route("/create-category")]
        public IActionResult CreateCategory([FromQuery] string CategoryId, string CategoryNameEn, string CategoryNameEs)
        {
            if(!string.IsNullOrEmpty(CategoryId))
            {
                ViewBag.CategoryId = CategoryId;
                ViewBag.CategoryNameEn = CategoryNameEn;
                ViewBag.CategoryNameEs = CategoryNameEs;
            }else
            {
                ViewBag.CategoryId = null;
                ViewBag.CategoryNameEn = null;
                ViewBag.CategoryNameEs = null;
            }
            return View();
        }

        [HttpPost]
        [Route("/create-category")]
        public async Task<IActionResult> CreateCategory(CategoryPostPutDto categoryPostPutDto)
        {
            if (categoryPostPutDto == null)
            {
                return View();
            }


            if (categoryPostPutDto.CategoryId == Guid.Empty)
            {
                
                await _blogService.CreateCategoryAsync(categoryPostPutDto);
            }
            else
            {
                await _blogService.UpdateCategory(categoryPostPutDto); 
            }


            return RedirectToAction("Categories");
        }


        // CREATE TAGS

        [Route("/tags")]
        public async Task<IActionResult> Tags()
        {
            IEnumerable<TagResponseDto> tags = await _blogService.GetAllTagsAsync();


            return View(tags);
        }

        [HttpGet]
        [Route("/create-tag")]
        public IActionResult CreateTag([FromQuery] string TagId, string TagNameEn, string TagNameEs)
        {
            if (!string.IsNullOrEmpty(TagId))
            {
                ViewBag.TagId = TagId;
                ViewBag.TagNameEn = TagNameEn;
                ViewBag.TagNameEs = TagNameEs;
            }
            else
            {
                ViewBag.TagId = null;
                ViewBag.TagNameEn = null;
                ViewBag.TagNameEs = null;
            }
            return View();
        }

        [HttpPost]
        [Route("/create-tag")]
        public async Task<IActionResult> CreateCategory(TagPostPutDto tagPostPutDto)
        {
            if (tagPostPutDto == null)
            {
                return View();
            }


            if (tagPostPutDto.TagId == Guid.Empty)
            {
                await _blogService.CreateTagAsync(tagPostPutDto);
            }
            else
            {
                await _blogService.UpdateTagAsync(tagPostPutDto);
            }


            return RedirectToAction("Tags");
        }

    }
}

using BlogMaster.Core.Contracts;
using BlogMaster.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogMaster.Controllers
{
    public class SiteController : Controller
    {

        private readonly IBlogService _blogService;
        private readonly IConfiguration _configuration;

        public SiteController(IBlogService blogService, IConfiguration configuration)
        {
            _configuration = configuration;
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

            ViewBag.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];
            ViewBag.Slug = slug;


            BlogResponseDto? blog = await _blogService.GetBlogBySlug(slug);

            if (blog == null)
            {
                return NotFound(); 
            }

            return View(blog);
        }

        
        [HttpGet]
        [Route("/create-comment")]
        public async Task<IActionResult> CreateComment([FromQuery] string blogId, string userId, string slug)
            {
            ViewBag.SignedIn = User.Identity?.IsAuthenticated;

            if(!ViewBag.SignedIn)
            {
                return RedirectToAction("SignIn", "Identity");
            }

            RatingResponseDto? ratingResponse = await _blogService.GetUserRatingOnBlog(Guid.Parse(blogId), Guid.Parse(userId));
            ViewBag.UserId = userId;
            ViewBag.BlogId = blogId;
            ViewBag.RatingId = ratingResponse?.RatingId;
            ViewBag.Slug = slug;

            return View(ratingResponse);
        }

        [Authorize]
        [HttpPost]
        [Route("/create-comment")]
        public async Task<IActionResult> CreateComment(RatingPostPutDto rating, CommentPostPutDto comment, [FromQuery] string slug)
        {

            //Handle rating
            if(rating.RatingScore != null && rating.RatingId == Guid.Empty)
            {
                await _blogService.AddRatingToBlogAsync(rating);
            }
            if(rating.RatingScore != null && rating.RatingId != Guid.Empty)
            {
                await _blogService.UpdateRatingAsync(rating);
            }
            if(rating.RatingScore == null && rating.RatingId != Guid.Empty)
            {
                await _blogService.DeleteRatingAsync(rating.RatingId);
            }

            if(!string.IsNullOrEmpty(comment.Message) && comment.CommmentId == Guid.Empty) 
            {
                await _blogService.AddCommentToBlogAsync(comment);
            }
            if (!string.IsNullOrEmpty(comment.Message) && comment.CommmentId != Guid.Empty)
            {
                await _blogService.UpdateCommentAsync(comment);
            }



            return RedirectToAction("BlogPage", new { slug = slug });
        }

        [HttpGet]
        [Route("/create-rating")]
        public async Task<IActionResult> CreateRating([FromQuery] string blogId, string userId, string slug)
        {
            ViewBag.SignedIn = User.Identity?.IsAuthenticated;

            if (!ViewBag.SignedIn)
            {
                return RedirectToAction("SignIn", "Identity");
            }

            RatingResponseDto? ratingResponse = await _blogService.GetUserRatingOnBlog(Guid.Parse(blogId), Guid.Parse(userId));
            ViewBag.UserId = userId;
            ViewBag.BlogId = blogId;
            ViewBag.RatingId = ratingResponse?.RatingId;
            ViewBag.Slug = slug;

            return View(ratingResponse);
        }

        [Authorize]
        [HttpPost]
        [Route("/create-rating")]
        public async Task<IActionResult> CreateRating(RatingPostPutDto rating, [FromQuery] string slug)
        {

            //Handle rating
            if (rating.RatingScore != null && rating.RatingId == Guid.Empty)
            {
                await _blogService.AddRatingToBlogAsync(rating);
            }
            if (rating.RatingScore != null && rating.RatingId != Guid.Empty)
            {
                await _blogService.UpdateRatingAsync(rating);
            }
            if (rating.RatingScore == null && rating.RatingId != Guid.Empty)
            {
                await _blogService.DeleteRatingAsync(rating.RatingId);
            }





            return RedirectToAction("BlogPage", new { slug = slug });
        }


    }
}

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
            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];

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
            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];

            if (pageIndex == 0)
            {
                pageIndex = 1;
            }

            if (string.IsNullOrEmpty(category) || category == "All")
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
        public async Task<IActionResult> BlogPage(string? slug = null, [FromQuery] string? category = "All")
        { 
            if (string.IsNullOrEmpty(slug))
            {
                return RedirectToAction("Blogs");
            }

            ViewBag.SignedIn = User.Identity?.IsAuthenticated;

            ViewBag.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];
            
            ViewBag.Slug = slug;

            ViewBag.Category = category;

            BlogResponseDto? blog = await _blogService.GetBlogBySlug(slug);



            BlogPreviewsDto? previews = await _blogService.GetBlogRecomendations(blog.Categories != null ? blog.Categories : new List<CategoryResponseDto>(), blog.BlogId.ToString());

            if (blog == null)
            {
                return NotFound(); 
            }

            BlogAndRecomendations blogAndRecomendations = new BlogAndRecomendations() 
            { 
                Blog = blog,
                BlogPreviews = previews
            };

            return View(blogAndRecomendations);
        }

        
        [HttpGet]
        [Route("/create-comment")]
        public async Task<IActionResult> CreateComment([FromQuery] string blogId, string userId, string slug)
        {
            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];

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
            ViewBag.UserName = User.Identity?.Name;

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

            if(!string.IsNullOrEmpty(comment.Message) && comment.CommentId == Guid.Empty) 
            {
                await _blogService.AddCommentToBlogAsync(comment);
            }
            if (!string.IsNullOrEmpty(comment.Message) && comment.CommentId != Guid.Empty)
            {
                await _blogService.UpdateCommentAsync(comment);
            }



            return RedirectToAction("BlogPage", new { slug = slug });
        }



        [HttpGet]
        [Route("/edit-comment")]
        public async Task<IActionResult> EditComment([FromQuery] string blogId, string userId, string slug, string commentId)
        {
            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];

            ViewBag.SignedIn = User.Identity?.IsAuthenticated;

            if (!ViewBag.SignedIn)
            {
                return RedirectToAction("SignIn", "Identity");
            }

            RatingResponseDto? ratingResponse = await _blogService.GetUserRatingOnBlog(Guid.Parse(blogId), Guid.Parse(userId));
            var comment = await _blogService.GetCommentAsync(Guid.Parse(commentId));
            ViewBag.Comment = comment.Message;
            ViewBag.CommentId = commentId;
            ViewBag.UserId = userId;
            ViewBag.BlogId = blogId;
            ViewBag.RatingId = ratingResponse?.RatingId;
            ViewBag.Slug = slug;

            return View(ratingResponse);
        }

        [Authorize]
        [HttpPost]
        [Route("/edit-comment")]
        public async Task<IActionResult> EditComment(RatingPostPutDto rating, CommentPostPutDto comment, [FromQuery] string slug)
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
            ViewBag.Slug = slug;

            if (comment.CommentId != Guid.Empty)
            {
                await _blogService.UpdateCommentAsync(comment);
            }



            return RedirectToAction("BlogPage", new { slug = slug });
        }


        [Route("/delete-comment")]
        public async Task<IActionResult> DeleteComment(string commentId, string slug)
        {

            if(!string.IsNullOrEmpty(commentId))
            {
                await _blogService.DeleteCommentAsync(Guid.Parse(commentId));
            }

            return RedirectToAction("BlogPage", new { slug = slug });

        }




        [HttpGet]
        [Route("/create-rating")]
        public async Task<IActionResult> CreateRating([FromQuery] string blogId, string userId, string slug)
        {
            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];

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

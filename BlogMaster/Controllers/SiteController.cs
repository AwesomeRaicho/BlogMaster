using BlogMaster.Core.Contracts;
using BlogMaster.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BlogMaster.Core.Services;
using BlogMaster.Core.Models;


namespace BlogMaster.Controllers
{
    public class SiteController : Controller
    {

        private readonly IBlogService _blogService;
        private readonly IConfiguration _configuration;
        private readonly CookieService _cookieService;
        private readonly IStripeService _stripeService;
        private readonly IIdentityService _identityService;
        private readonly string _currencyCode;
        public SiteController(IBlogService blogService, IConfiguration configuration, CookieService cookieService, IStripeService stripeService, IIdentityService identityService)
        {
            _configuration = configuration;
            _blogService = blogService;
            _cookieService = cookieService;
            _stripeService = stripeService;
            _identityService = identityService;
            _currencyCode = configuration["CurrencyCode:Peso"] ?? "";

        }

        [Route("/")]
        public IActionResult Index()
        {
            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];
            ViewBag.CurrencyCode = _currencyCode.ToUpper();
            ViewBag.Title = "Blog Master";
            ViewBag.SignedIn = User.Identity?.IsAuthenticated;
            string? user = User.Identity?.Name;


            if (ViewBag.SignedIn)
            {
                ViewBag.UserName = User.Identity?.Name;
                ViewBag.UserEmail = User.FindFirstValue(ClaimTypes.Email);
            }

            if (User.IsInRole("Administrator"))
            {
                return RedirectToAction("AdminBlogViews", "Blog");
            }
            return View();
        }

        [Route("/blogs")]
        public async Task<IActionResult> Blogs([FromQuery] int pageIndex, string category, List<string> tags, string? search )
        {
            ViewBag.CurrencyCode = _currencyCode.ToUpper();

            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];
            if (User.IsInRole("Administrator"))
            {
                ViewBag.IsAdmin = true;
            }
            else
            {
                ViewBag.IsAdmin = false;
            }

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


            //creating a new dictionary to accommodate preview parameters
            Dictionary<string, string> filters = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(search))
            {
                filters.Add("search", search);
            }

            var previews = await _blogService.GetAllBlogPreviews(pageIndex, category, tags.Count != 0 && tags[0] != null ? tags : new List<string>(), filters, null);

            return View(previews);
        }


        [Route("/blogs/blogpage/{slug?}")]
        public async Task<IActionResult> BlogPage(string? slug = null, [FromQuery] string? category = "All")
        {
            ViewBag.CurrencyCode = _currencyCode.ToUpper();

            //Logic for admin
            if (User.IsInRole("Administrator"))
            {
                ViewBag.IsAdmin = true;
                
                ViewBag.SignedIn = User.Identity?.IsAuthenticated;

                ViewBag.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];

                ViewBag.Slug = slug;

                ViewBag.Category = category;
                BlogResponseDto? blogadmin = await _blogService.GetBlogBySlug(slug ?? "");

                BlogPreviewsDto? previewssubadmin = await _blogService.GetBlogRecomendations(blogadmin.Categories != null ? blogadmin.Categories : new List<CategoryResponseDto>(), blogadmin.BlogId.ToString());

                if (blogadmin == null)
                {
                    return NotFound();
                }

                BlogAndRecomendations blogAndRecomendationssub = new BlogAndRecomendations()
                {
                    Blog = blogadmin,
                    BlogPreviews = previewssubadmin
                };

                return View(blogAndRecomendationssub);
            }
            else
            {
                ViewBag.IsAdmin = false;
            }


            //Rest of logic is for non admins

            if (string.IsNullOrEmpty(slug))
            {
                return RedirectToAction("Blogs");
            }
            BlogResponseDto? blog = await _blogService.GetBlogBySlug(slug);

            if (blog.IsSubscriptionRequired != null && blog.IsSubscriptionRequired == true)
            {
                //check if user is signed in
                if(User.Identity != null && User.Identity.IsAuthenticated)
                {
                    string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    
                    //check if already contains activbe cookie
                    string? subed = _cookieService.Unprotect(Request.Cookies["subed"]);

                    if (string.IsNullOrEmpty(subed) && !string.IsNullOrEmpty(userId))
                    {
                        //get StripeCustomerId from DB
                        string? stripeCustomerId = await _identityService.GetStripeCustomerId(userId);


                        if (string.IsNullOrEmpty(stripeCustomerId))
                        {
                            //if theres no StripId it means no subscription created 

                            return RedirectToAction("SubscriptionDetails", "Subscription");
                        }

                        string? status = await _stripeService.SubscriptionStatus(stripeCustomerId);

                        if(status != null && status == "active")
                        {
                            //create cookie
                            string? cookieValue = _cookieService.Protect("active");
                            if (!string.IsNullOrEmpty(cookieValue))
                            {
                                Response.Cookies.Append("subed", cookieValue, new CookieOptions { HttpOnly = true, Secure = true, Expires = DateTime.UtcNow.AddDays(2) });
                            }

                            ViewBag.SignedIn = User.Identity?.IsAuthenticated;

                            ViewBag.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];

                            ViewBag.Slug = slug;

                            ViewBag.Category = category;

                            BlogPreviewsDto? previewssub = await _blogService.GetBlogRecomendations(blog.Categories != null ? blog.Categories : new List<CategoryResponseDto>(), blog.BlogId.ToString());

                            if (blog == null)
                            {
                                return NotFound();
                            }

                            BlogAndRecomendations blogAndRecomendationssub = new BlogAndRecomendations()
                            {
                                Blog = blog,
                                BlogPreviews = previewssub
                            };

                            return View(blogAndRecomendationssub);

                        }
                        else
                        {
                            //if status is not active the need to subscribe
                            return RedirectToAction("SubscriptionDetails", "Subscription");
                        }

                    }
                }
                else
                {
                    //redirect to sign in page if not signed in
                    return RedirectToAction("SignIn", "Identity");
                }
            }



            ViewBag.SignedIn = User.Identity?.IsAuthenticated;

            ViewBag.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];
            
            ViewBag.Slug = slug;

            ViewBag.Category = category;

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
            ViewBag.CurrencyCode = _currencyCode.ToUpper();

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
            ViewBag.CurrencyCode = _currencyCode.ToUpper();

            //Handle rating
            if (rating.RatingScore != null && rating.RatingId == Guid.Empty)
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
            ViewBag.CurrencyCode = _currencyCode.ToUpper();

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
            ViewBag.CurrencyCode = _currencyCode.ToUpper();

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
            ViewBag.CurrencyCode = _currencyCode.ToUpper();

            if (!string.IsNullOrEmpty(commentId))
            {
                await _blogService.DeleteCommentAsync(Guid.Parse(commentId));
            }
            return RedirectToAction("BlogPage", new { slug = slug });
        }




        [HttpGet]
        [Route("/create-rating")]
        public async Task<IActionResult> CreateRating([FromQuery] string blogId, string userId, string slug)
        {
            ViewBag.CurrencyCode = _currencyCode.ToUpper();

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
            ViewBag.CurrencyCode = _currencyCode.ToUpper();

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

        [Authorize]
        [HttpGet]
        [Route("/profile")]
        public IActionResult Profile()
        {
            ViewBag.CurrencyCode = _currencyCode.ToUpper();

            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];
            ViewBag.Title = "Profile";
            ViewBag.SignedIn = User.Identity?.IsAuthenticated;

            ViewBag.UserName = User.Identity?.Name;
            ViewBag.Email = User.FindFirstValue(ClaimTypes.Email);

            //ViewBag.IsAdmin
            if (User.IsInRole("Administrator"))
            {
                ViewBag.IsAdmin = true;
            }
            else
            {
                ViewBag.IsAdmin = false;
            }

            return View();
        }

        [Route("/blogs/blogpageId")]
        public async Task<IActionResult> BlogById(string blogId)
        {
            ViewBag.CurrencyCode = _currencyCode.ToUpper();

            ViewBag.IsAdmin = true;

            ViewBag.SignedIn = User.Identity?.IsAuthenticated;

            ViewBag.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ViewBag.FontAwesomeKey = _configuration["FontAwesome:Key"];
            ViewBag.Slug = "";

            ViewBag.Category = "";

            var blog = await _blogService.GetBlogByIdAsync(Guid.Parse(blogId));

            BlogPreviewsDto? previewssubadmin = await _blogService.GetBlogRecomendations(blog.Categories != null ? blog.Categories : new List<CategoryResponseDto>(), blog.BlogId.ToString());

            if (blog == null)
            {
                return NotFound();
            }

            BlogAndRecomendations blogAndRecomendationssub = new BlogAndRecomendations()
            {
                Blog = blog,
                BlogPreviews = previewssubadmin
            };

            return View(blogAndRecomendationssub);
        }

    }
}

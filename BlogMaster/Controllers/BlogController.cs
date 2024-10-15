using BlogMaster.Core.Contracts;
using BlogMaster.Core.DTO;
using BlogMaster.Core.Models;
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
        [Route("/delete-tag")]
        public async Task<IActionResult> DeleteTag([FromQuery] string tagId)
        {
            if(string.IsNullOrEmpty(tagId))
            {
                return RedirectToAction("Tags");
            }



            await _blogService.DeleteTagAsync(Guid.Parse(tagId));
            return RedirectToAction("Tags");


        }

        [HttpGet]
        [Route("/delete-blog")]
        public async Task<IActionResult> DeleteBlog([FromQuery] string blogId)
        {
            if(string.IsNullOrEmpty(blogId))
            {
                return RedirectToAction("AdminBlogViews");
            }

            await _blogService.DeleteBlogAsync(Guid.Parse(blogId));
            //await _blogService.delete



            return RedirectToAction("AdminBlogViews");
        }


        [HttpGet]
        [Route("/create-blog")]
        public async Task<IActionResult> CreateBlog([FromQuery] string blogId)
        {
            BlogEditViewDto blogEdit = new BlogEditViewDto();

            if(!string.IsNullOrWhiteSpace(blogId))
            {
            
                blogEdit.BlogResponseDto = await _blogService.GetBlogByIdAsync(Guid.Parse(blogId));


            }else
            {
                blogEdit.BlogResponseDto = new BlogResponseDto();
            }


            blogEdit.AllCategories = (List<CategoryResponseDto>)await _blogService.GetAllCategories();
            blogEdit.AllTags = (List<TagResponseDto>)await _blogService.GetAllTagsAsync();
            blogEdit.AllKeywords = (List<KeywordResponseDto>)await _blogService.GetAllKeywordsAsync();


            return View(blogEdit);
        }

        [HttpPost]
        [Route("/create-blog")]
        public async Task<IActionResult> CreateBlog(BlogPostPutDto blogPost, List<IFormFile> blogImage)
        {
            
            if (blogPost.CategoriesIds == null) 
            {
                blogPost.CategoriesIds = new List<string>();
            }
            if (blogPost.TagsIds == null) 
            {
                blogPost.TagsIds = new List<string>();
            }
            if (blogPost.KeywordsIds == null) 
            {
                blogPost.KeywordsIds = new List<string>();
            }
            

            

            string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null)
            {
                return RedirectToAction("SignIn");
            }

            blogPost.UserId = Guid.Parse(id);

            if(blogPost.BlogId == Guid.Empty)
            {
                await _blogService.CreateBlogAsync(blogPost);
            }else
            {
                await _blogService.UpdateBlogAsync(blogPost);
            }


            return RedirectToAction("AdminBlogViews");
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
            ViewBag.Tags = tags.Count != 0 && tags[0] != null ? tags : null;


            var previews = await _blogService.GetAllAdminBlogPreviews(pageIndex, category, tags.Count != 0 && tags[0] != null ? tags : new List<string>());

            return View(previews);
        }



        //CATEGORIES
        [Route("/categories")]
        public async Task<IActionResult> Categories()
        {
            IEnumerable<CategoryResponseDto> categories = await _blogService.GetAllCategories();
            return View(categories);
        }

        [HttpGet]
        [Route("/delete-category")]
        public async Task<IActionResult> DeleteCategory([FromQuery] string categoryId)
        {
            if(string.IsNullOrEmpty(categoryId))
            {
                return RedirectToAction("Categories");
            }
            await _blogService.DeleteCategoryAsync(Guid.Parse(categoryId));


            return RedirectToAction("Categories");


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


        // TAGS

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

        // KEYWORDS
        [Route("/keywords")]
        public async Task<IActionResult> Keywords()
        {
            IEnumerable<KeywordResponseDto> keywords = await _blogService.GetAllKeywordsAsync();

            return View(keywords);
        }

        [HttpGet]
        [Route("/delete-keyword")]
        public async Task<IActionResult> DeleteKeyword([FromQuery] string keywordId)
        {
            if(string.IsNullOrEmpty(keywordId))
            {
                return RedirectToAction("Keywords");
            }

            await _blogService.DeleteKeywordAsync(Guid.Parse(keywordId));

            return RedirectToAction("Keywords");


        }


        [HttpGet]
        [Route("/create-keyword")]
        public IActionResult CreateKeyword([FromQuery] string KeywordId, string KeywordNameEn, string KeywordNameEs)
        {
            if (!string.IsNullOrEmpty(KeywordId))
            {
                ViewBag.KeywordId = KeywordId;
                ViewBag.KeywordNameEn = KeywordNameEn;
                ViewBag.KeywordNameEs = KeywordNameEs;
            }
            else
            {
                ViewBag.KeywordId = null;

                ViewBag.KeywordNameEn = null;
                ViewBag.KeywordNameEs = null;
            }
            return View();
        }

        [HttpPost]
        [Route("/create-keyword")]
        public async Task<IActionResult> CreateKeyword(KeywordPostPutDto keywordPostPutDto)
        {
            if (keywordPostPutDto == null)
            {
                return View();
            }


            if (keywordPostPutDto.KeywordId == Guid.Empty)
            {
                await _blogService.CreateKeywordAsync(keywordPostPutDto);
            }
            else
            {
                await _blogService.UpdateKeywordAsync(keywordPostPutDto);
            }


            return RedirectToAction("Keywords");
        }

        [HttpGet]
        [Route("/add-blog-image")]
        public async Task<IActionResult> AddBlogImage([FromQuery] BlogPostPutDto blogDto)
        {
            List<BlogImagesResponseDto> images = await _blogService.GetAllBlogImages(blogDto.BlogId.ToString());
            List<string> srcs = new List<string>();

            foreach(BlogImagesResponseDto img in images)
            {
                if(img.ImageData != null)
                {
                    string base64String = Convert.ToBase64String(img.ImageData);
                    string imgSrc = $"data:{img.MimeType};base64,{base64String}";
                    srcs.Add(imgSrc);
                }
            }

            blogDto.ImageSrcs = srcs;



            return View(blogDto);
        }


        [HttpPost]
        [Route("/add-blog-image")]
        public async Task<IActionResult> AddBlogImage(BlogPostPutDto blogDto, List<IFormFile> newImages, List<string> deletedImages)
        {

            if (newImages.Count != 0)
            {
                foreach (IFormFile image in newImages)
                {
                    if (image != null && image.Length > 0)
                    {
                        BlogImagePostPutDto newImageDto = new BlogImagePostPutDto();
                        using (MemoryStream str = new MemoryStream())
                        {
                            await image.CopyToAsync(str);
                            newImageDto.ImageData = str.ToArray();
                            newImageDto.ImageName = image.FileName;
                            newImageDto.MimeType = image.ContentType;
                            newImageDto.BlogId = blogDto.BlogId;
                        }

                        await _blogService.AddImageToBlogAsync(newImageDto);
                    }
                }
            }



            return RedirectToAction("CreateBlog", new { blogId = blogDto.BlogId });
        }

    }
}

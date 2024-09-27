using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; 
using Microsoft.AspNetCore.Mvc;

public class BlogController : Controller
{
    private readonly IBlogRepository _blogRepository;
    private readonly ICommentsRepository _commentsRepository; 
    private readonly UserManager<User> _userManager;

    public BlogController(IBlogRepository blogRepository, ICommentsRepository commentsRepository, UserManager<User> userManager)
    {
        _blogRepository = blogRepository;
        _commentsRepository = commentsRepository; 
        _userManager = userManager;
    }

    [Authorize]
    [HttpGet]
    public IActionResult MakeBlog()
    {
        return View(new MakeBlogViewModel());
    }

    [HttpGet]
    public IActionResult ViewBlog(int id)
    {
        var blog = _blogRepository.GetBlogById(id);
    
        if (blog == null)
        {
            return NotFound(); 
        }

        return View(blog);
    }

    [HttpGet]
    public IActionResult Comment(int id)
    {
        var blog = _blogRepository.GetBlogById(id);
        if (blog == null)
        {
            return NotFound();
        }

        var comments = _commentsRepository.GetCommentsByBlogId(id)
                                        .OrderByDescending(c => c.CreatedAt)
                                        .AsEnumerable(); 

        return View((blog, comments));
    }

    [HttpPost]
    public async Task<IActionResult> SubmitComment(int id, string CommentContent)
    {
        var blog = _blogRepository.GetBlogById(id);
        if (blog == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);

        var comment = new Comment
        {
            BlogId = blog.Id,
            UserId = user.Id,  
            Content = CommentContent,
            CreatedAt = DateTime.Now
        };

        _commentsRepository.AddComment(comment);

        return RedirectToAction("Comment", new { id = blog.Id });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateBlog(MakeBlogViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);

            string? profilePicturePath = null;
            if (model.ProfilePicture != null)
            {
                var profilePicFileName = Path.GetFileName(model.ProfilePicture.FileName);
                var profilePicFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", profilePicFileName);
                using (var stream = new FileStream(profilePicFilePath, FileMode.Create))
                {
                    await model.ProfilePicture.CopyToAsync(stream);
                }
                profilePicturePath = "/uploads/" + profilePicFileName;
            }


            var imagePaths = new List<string>();
            if (model.Images != null && model.Images.Count > 0)
            {
                foreach (var image in model.Images)
                {
                    var imageFileName = Path.GetFileName(image.FileName);
                    var imageFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", imageFileName);
                    using (var stream = new FileStream(imageFilePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                    imagePaths.Add("/uploads/" + imageFileName);
                }
            }

            var blog = new Blog
            {
                Title = model.Title,
                Description = model.Description,
                Author = user,
                ProfilePicture = profilePicturePath, 
                Posts = new List<Post>(),
            };

            _blogRepository.AddBlog(blog);
            return RedirectToAction("Index", "Home"); 
        }

        return View("MakeBlog", model);
    }
}

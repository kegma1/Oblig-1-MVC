using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; 
using Microsoft.AspNetCore.Mvc;

public class BlogController : Controller
{
    private readonly IBlogRepository _blogRepository;
    private readonly ICommentsRepository _commentsRepository; 
    private readonly IPostRepository _postRepository;
    private readonly UserManager<User> _userManager;

    public BlogController(IBlogRepository blogRepository, ICommentsRepository commentsRepository, IPostRepository postRepository,UserManager<User> userManager)
    {
        _blogRepository = blogRepository;
        _commentsRepository = commentsRepository; 
        _userManager = userManager;
        _postRepository = postRepository;
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

        var viewModel = new ViewBlogViewModel {
            Blog = blog,
            MakePostViewModel = new MakePostViewModel()
        };

        return View(viewModel);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> MakePost(ViewBlogViewModel model) {

        string? optionalImagePath = null;
        if (model.MakePostViewModel.Image != null)
        {
            var profilePicFileName = Path.GetFileName(model.MakePostViewModel.Image.FileName);
            var profilePicFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", profilePicFileName);
            using (var stream = new FileStream(profilePicFilePath, FileMode.Create))
            {
                await model.MakePostViewModel.Image.CopyToAsync(stream);
            }
            optionalImagePath = "/uploads/" + profilePicFileName;
        }

        var blog = _blogRepository.GetBlogById(model.MakePostViewModel.blogId);

        var newPost = new Post {
            Title = model.MakePostViewModel.Title,
            Content = model.MakePostViewModel.Content,
            CreatedAt = DateTime.Now,
            Author = blog.Author,
            blog = blog,
            Image = optionalImagePath
        };

        _postRepository.AddPost(newPost);

        return RedirectToAction("viewBlog", "Blog", new { id = blog.Id });
      
    }

    [HttpPost]
    public async Task<IActionResult> SubmitComment(int postId, string commentContent)
    {
        var post = _postRepository.GetPostById(postId);
        if (post == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);

        var comment = new Comment
        {
            PostId = post.Id,
            UserId = user.Id,
            Content = commentContent,
            CreatedAt = DateTime.Now
        };

        _commentsRepository.AddComment(comment);

        return RedirectToAction("CommentOnPost", new { postId = post.Id });
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

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> EditBlog(int blogId) {
        var blog = _blogRepository.GetBlogById(blogId);

        if (blog == null) {
            return NotFound("User not found");
        }

        var editBlogViewModel = new EditBlogViewModel {
            Title = blog.Title,
            Description = blog.Description,
            BannerUrl = blog.ProfilePicture,
            blogId = blog.Id,
        };
        

        return View(editBlogViewModel);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> EditBlog(EditBlogViewModel model) {
        if (ModelState.IsValid) {
            var blog = _blogRepository.GetBlogById(model.blogId);


            if (blog == null) {
                return NotFound();
            }
            if (model.NewBanner != null) {
                
                var profilePicFileName = Path.GetFileName(model.NewBanner.FileName);
                var profilePicFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", profilePicFileName);
                using (var stream = new FileStream(profilePicFilePath, FileMode.Create))
                {
                    await model.NewBanner.CopyToAsync(stream);
                }
                blog.ProfilePicture = "/uploads/" + profilePicFileName;
            }

            if (model.Description != null) {
                blog.Description = model.Description;
            }

            if (model.Title != null) {
                blog.Title = model.Title;
            }

            _blogRepository.UpdateBlog(blog);

            return RedirectToAction("ViewBlog", "Blog",  new { id = model.blogId });
        }

        return View(model);
    }
    [HttpGet]
    public IActionResult CommentOnPost(int postId)
    {
        var post = _postRepository.GetPostById(postId);
        if (post == null)
        {
            return NotFound();
        }

        var comments = _commentsRepository.GetCommentsByPostId(postId)
                                        .OrderByDescending(c => c.CreatedAt)
                                        .AsEnumerable();

        var model = new CommentViewModel 
        {
            Post = post,
            Comments = comments
        };

        return View("Comment", model); 
    }
    [HttpPost]
    [Authorize]
    public IActionResult DeletePost(int postId)
    {
        var post = _postRepository.GetPostById(postId);
        if (post == null)
        {
            return NotFound();
        }

        if (post.Author.Id != _userManager.GetUserId(User))
        {
            return Forbid();
        }

        _postRepository.DeletePost(postId);
        return RedirectToAction("ViewBlog", new { id = post.blog.Id });
    }

    [HttpPost]
    [Authorize]
    public IActionResult DeleteBlog(int blogId)
    {
        var blog = _blogRepository.GetBlogById(blogId);
        if (blog == null)
        {
            return NotFound();
        }

        if (blog.Author.Id != _userManager.GetUserId(User))
        {
            return Forbid();
        }

        _blogRepository.DeleteBlog(blogId);
        return RedirectToAction("Index", "Home");
    }

}


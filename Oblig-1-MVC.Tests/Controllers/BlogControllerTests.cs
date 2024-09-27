using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

public class BlogControllerTests
{
    private readonly Mock<IBlogRepository> _blogRepositoryMock;
    private readonly Mock<ICommentsRepository> _commentsRepositoryMock;
    private readonly Mock<IPostRepository> _postRepositoryMock;
    private readonly Mock<UserManager<User>> _userManagerMock;

    public BlogControllerTests()
    {
        _blogRepositoryMock = new Mock<IBlogRepository>();
        _commentsRepositoryMock = new Mock<ICommentsRepository>();
        _postRepositoryMock = new Mock<IPostRepository>();

        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
    }

    [Fact]
    public void MakeBlog_ReturnsViewResult()
    {
        var controller = new BlogController(_blogRepositoryMock.Object, _commentsRepositoryMock.Object, _postRepositoryMock.Object, _userManagerMock.Object);
        var result = controller.MakeBlog();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void ViewBlog_ReturnsNotFound_WhenBlogNotFound()
    {
        _blogRepositoryMock.Setup(b => b.GetBlogById(It.IsAny<int>())).Returns((Blog)null);
        var controller = new BlogController(_blogRepositoryMock.Object, _commentsRepositoryMock.Object, _postRepositoryMock.Object, _userManagerMock.Object);
        var result = controller.ViewBlog(1);
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task MakePost_RedirectsToAction_WhenSuccessful()
    {
        var controller = new BlogController(_blogRepositoryMock.Object, _commentsRepositoryMock.Object, _postRepositoryMock.Object, _userManagerMock.Object);
        var viewModel = new ViewBlogViewModel
        {
            MakePostViewModel = new MakePostViewModel { blogId = 1, Title = "New Post", Content = "Content" }
        };
        _blogRepositoryMock.Setup(b => b.GetBlogById(It.IsAny<int>())).Returns(new Blog());
        var result = await controller.MakePost(viewModel);
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("viewBlog", redirectResult.ActionName);
    }

    [Fact]
    public async Task SubmitComment_RedirectsToAction_WhenSuccessful()
    {
        var post = new Post { Id = 1 };
        _postRepositoryMock.Setup(p => p.GetPostById(It.IsAny<int>())).Returns(post);
        _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(new User());
        var controller = new BlogController(_blogRepositoryMock.Object, _commentsRepositoryMock.Object, _postRepositoryMock.Object, _userManagerMock.Object);
        var result = await controller.SubmitComment(1, "Nice post!");
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("CommentOnPost", redirectResult.ActionName);
    }

    [Fact]
    public async Task CreateBlog_RedirectsToAction_WhenModelStateIsValid()
    {
        var model = new MakeBlogViewModel { Title = "My Blog", Description = "This is my blog." };
        _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).ReturnsAsync(new User());
        var controller = new BlogController(_blogRepositoryMock.Object, _commentsRepositoryMock.Object, _postRepositoryMock.Object, _userManagerMock.Object);
        var result = await controller.CreateBlog(model);
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }

    [Fact]
    public async Task EditBlog_Post_UpdatesBlog_WhenModelStateIsValid()
    {
        var model = new EditBlogViewModel { blogId = 1, Title = "Updated Blog", Description = "Updated description." };
        var blog = new Blog { Id = 1 };
        _blogRepositoryMock.Setup(b => b.GetBlogById(It.IsAny<int>())).Returns(blog);
        var controller = new BlogController(_blogRepositoryMock.Object, _commentsRepositoryMock.Object, _postRepositoryMock.Object, _userManagerMock.Object);
        var result = await controller.EditBlog(model);
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("ViewBlog", redirectResult.ActionName);
    }

    [Fact]
    public void DeletePost_RedirectsToAction_WhenSuccessful()
    {
        var post = new Post { Id = 1, blog = new Blog { Id = 2 }, Author = new User { Id = "authorId" } };
        _postRepositoryMock.Setup(p => p.GetPostById(It.IsAny<int>())).Returns(post);
        _userManagerMock.Setup(u => u.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns("authorId");
        var controller = new BlogController(_blogRepositoryMock.Object, _commentsRepositoryMock.Object, _postRepositoryMock.Object, _userManagerMock.Object);
        var result = controller.DeletePost(1);
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("ViewBlog", redirectResult.ActionName);
    }

    [Fact]
    public void DeleteBlog_RedirectsToAction_WhenSuccessful()
    {
        var blog = new Blog { Id = 1, Author = new User { Id = "authorId" } };
        _blogRepositoryMock.Setup(b => b.GetBlogById(It.IsAny<int>())).Returns(blog);
        _userManagerMock.Setup(u => u.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns("authorId");
        var controller = new BlogController(_blogRepositoryMock.Object, _commentsRepositoryMock.Object, _postRepositoryMock.Object, _userManagerMock.Object);
        var result = controller.DeleteBlog(1);
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }
}

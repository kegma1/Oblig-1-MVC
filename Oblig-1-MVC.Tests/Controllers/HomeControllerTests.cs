using Moq;
using oblig1.Controllers;
using oblig1.Models;
using Microsoft.AspNetCore.Mvc;

public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _loggerMock;
    private readonly Mock<IBlogRepository> _blogRepositoryMock;

    public HomeControllerTests()
    {
        _loggerMock = new Mock<ILogger<HomeController>>();
        _blogRepositoryMock = new Mock<IBlogRepository>();
    }

    [Fact]
    public void Index_ReturnsViewResult_WithBlogs()
    {
        _blogRepositoryMock.Setup(b => b.GetAllBlogs()).Returns(new List<Blog>());
        var controller = new HomeController(_loggerMock.Object, _blogRepositoryMock.Object);
        var result = controller.Index();
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<IndexViewModel>(viewResult.Model);
    }

    [Fact]
    public void NewHome_ReturnsViewResult()
    {
        var controller = new HomeController(_loggerMock.Object, _blogRepositoryMock.Object);
        var result = controller.NewHome();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Privacy_ReturnsViewResult()
    {
        var controller = new HomeController(_loggerMock.Object, _blogRepositoryMock.Object);
        var result = controller.Privacy();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Error_ReturnsViewResult_WithErrorViewModel()
    {
        var controller = new HomeController(_loggerMock.Object, _blogRepositoryMock.Object);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();
        controller.HttpContext.TraceIdentifier = "test_trace_id";

        var result = controller.Error();
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ErrorViewModel>(viewResult.Model);

        Assert.Equal("test_trace_id", model.RequestId);
    }
}

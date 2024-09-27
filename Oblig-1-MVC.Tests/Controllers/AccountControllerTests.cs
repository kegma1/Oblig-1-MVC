using Moq;
using oblig1.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MockQueryable.Moq;


public class AccountControllerTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<SignInManager<User>> _signInManagerMock;
    private readonly Mock<IBlogRepository> _blogRepositoryMock;

    public AccountControllerTests()
    {
        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

        var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
        _signInManagerMock = new Mock<SignInManager<User>>(
            _userManagerMock.Object,
            contextAccessor.Object,
            claimsFactory.Object,
            null,
            null,
            null,
            null
        );

        _blogRepositoryMock = new Mock<IBlogRepository>();
    }

    private void SetupMockUsers()
    {
        _userManagerMock.Setup(u => u.Users).Returns(GetMockUsersAsAsyncQueryable());
    }

    private IQueryable<User> GetMockUsersAsAsyncQueryable()
    {
        var users = new List<User>
        {
            new User { Id = "currentUser", UserName = "TestUser", Following = new List<User>(), Followers = new List<User>() },
            new User { Id = "otherUser", UserName = "OtherUser", Following = new List<User>(), Followers = new List<User>() }
        };

        return new TestAsyncEnumerable<User>(users);
    }


    [Fact]
    public void Register_ReturnsViewResult()
    {
        var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _blogRepositoryMock.Object);
        var result = controller.Register();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Register_Post_ReturnsRedirectToAction_WhenModelStateIsValid()
    {
        var model = new RegisterViewModel { Username = "testuser", Email = "test@example.com", Password = "Test123!" };
        var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _blogRepositoryMock.Object);
        _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        var result = await controller.Register(model);
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }

    [Fact]
    public async Task Register_Post_ReturnsViewResult_WhenModelStateIsInvalid()
    {
        var model = new RegisterViewModel();
        var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _blogRepositoryMock.Object);
        controller.ModelState.AddModelError("error", "some error");
        var result = await controller.Register(model);
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Login_ReturnsViewResult()
    {
        var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _blogRepositoryMock.Object);
        var result = controller.Login();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Login_Post_ReturnsRedirectToAction_WhenLoginSuccessful()
    {
        var model = new LoginViewModel { Username = "testuser", Password = "password", RememberMe = false };
        var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _blogRepositoryMock.Object);
        _signInManagerMock.Setup(s => s.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
        var result = await controller.Login(model);
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }

    [Fact]
    public async Task Login_Post_ReturnsViewResult_WhenLoginFails()
    {
        var model = new LoginViewModel { Username = "testuser", Password = "wrongpassword", RememberMe = false };
        var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _blogRepositoryMock.Object);
        _signInManagerMock.Setup(s => s.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);
        var result = await controller.Login(model);
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Logout_ReturnsRedirectToAction()
    {
        var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _blogRepositoryMock.Object);
        var result = await controller.Logout();
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }

    [Fact]
    public async Task PublicProfile_ReturnsViewResult_WithValidUserId()
    {
        SetupMockUsers();
        var userId = "currentUser";
        var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _blogRepositoryMock.Object);
        var result = await controller.PublicProfile(userId);
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task EditProfile_ReturnsViewResult()
    {
        var user = new User { UserName = "testuser", Bio = "test bio" };
        _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _blogRepositoryMock.Object);
        var result = await controller.EditProfile();
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task EditProfile_Post_UpdatesProfile_WhenModelStateIsValid()
    {
        var model = new EditProfileViewModel { Username = "testuser", Bio = "updated bio" };
        var user = new User { UserName = "testuser", Bio = "old bio" };
        _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
        var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _blogRepositoryMock.Object);
        var result = await controller.EditProfile(model);
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }

    [Fact]
    public async Task FollowUser_ReturnsBadRequest_WhenUserDoesNotExist()
    {
        var currentUser = new User { Id = "currentUser", Following = new List<User>() };
        var users = new List<User>().AsQueryable().BuildMock();
        _userManagerMock.Setup(u => u.Users).Returns(users);
        _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
        var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _blogRepositoryMock.Object);
        var result = await controller.FollowUser("nonExistentUser");
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task FollowUser_ReturnsBadRequest_WhenUserIsSameAsUserToFollow()
    {  
        var currentUser = new User { Id = "currentUser", Following = new List<User>() };
        var users = new List<User> { currentUser }.AsQueryable().BuildMock();
        _userManagerMock.Setup(u => u.Users).Returns(users);
        _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
        var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _blogRepositoryMock.Object);
        var result = await controller.FollowUser("currentUser");
        Assert.IsType<BadRequestObjectResult>(result);
    }


    [Fact]
    public async Task UnfollowUser_ReturnsBadRequest_WhenUserDoesNotExist()
    {
        var currentUser = new User { Id = "currentUser", Following = new List<User>() };
        var users = new List<User>().AsQueryable().BuildMock();
        _userManagerMock.Setup(u => u.Users).Returns(users);
        _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(currentUser);
        var controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _blogRepositoryMock.Object);
        var result = await controller.UnfollowUser("nonExistentUser");
        Assert.IsType<BadRequestObjectResult>(result);
    }
}

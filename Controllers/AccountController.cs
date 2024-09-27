using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace oblig1.Controllers
{
    public class AccountController : Controller {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IBlogRepository _blogRepository;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IBlogRepository blogRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _blogRepository = blogRepository;
        }

        [HttpGet]
        public IActionResult Register() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model) {
            if (ModelState.IsValid) {

                var user = new User {
                    UserName = model.Username,
                    Email = model.Email,
                };

                var res = await _userManager.CreateAsync(user, model.Password);

                if (res.Succeeded) {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var err in res.Errors) {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var res = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (res.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Invalid login");
            }
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout() {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> PublicProfile(string userId)
        {
            var user = await _userManager.Users
                .Include(u => u.Followers)
                .Include(u => u.Following)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var profileViewModel = new PublicProfileViewModel
            {
                UserId = user.Id,
                Username = user.UserName,
                Bio = user.Bio,
                ProfilePictureUrl = user.ProfilePicture,
                FollowerCount = user.Followers.Count,
                FollowingCount = user.Following.Count,
                Blogs = _blogRepository.GetAllBlogsByAuthor(userId).ToList()
            };

            return View(profileViewModel);
        }







        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProfile() {
            var user = await _userManager.GetUserAsync(User);

            if (user == null) {
                return NotFound("User not found");
            }

            var editProfileViewModel = new EditProfileViewModel {
                Username = user.UserName,
                Bio = user.Bio,
                ProfilePictureUrl = user.ProfilePicture,
            };
            

            return View(editProfileViewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model) {
            if (ModelState.IsValid) {
                var user = await _userManager.GetUserAsync(User);

                if (user == null) {
                    return NotFound();
                }
                if (model.NewProfilePicture != null) {
                    var profilePicFileName = Path.GetFileName(model.NewProfilePicture.FileName);
                    var profilePicFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", profilePicFileName);
                    using (var stream = new FileStream(profilePicFilePath, FileMode.Create))
                    {
                        await model.NewProfilePicture.CopyToAsync(stream);
                    }
                    user.ProfilePicture = profilePicFileName;
                }

                if (model.Bio != null) {
                    user.Bio = model.Bio;
                }

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded) {
                    return RedirectToAction("Index", "Home"); 
                }

                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> FollowUser(string userId)
        {
            var user = await _userManager.Users
                .Include(u => u.Following)
                .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));
            var userToFollow = await _userManager.Users
                .Include(u => u.Followers)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || userToFollow == null || user.Id == userToFollow.Id)
            {
                return BadRequest("Invalid follow request.");
            }

            if (!user.Following.Any(f => f.Id == userToFollow.Id))
            {
                user.Following.Add(userToFollow);
                userToFollow.Followers.Add(user);

                await _userManager.UpdateAsync(user);
                await _userManager.UpdateAsync(userToFollow);
            }

            return RedirectToAction("PublicProfile", new { userId = userToFollow.Id });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UnfollowUser(string userId)
        {
            var user = await _userManager.Users
                .Include(u => u.Following)
                .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));
            var userToUnfollow = await _userManager.Users
                .Include(u => u.Followers)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || userToUnfollow == null || user.Id == userToUnfollow.Id)
            {
                return BadRequest("Invalid unfollow request.");
            }

            if (user.Following.Any(f => f.Id == userToUnfollow.Id))
            {
                user.Following.Remove(userToUnfollow);
                userToUnfollow.Followers.Remove(user);

                await _userManager.UpdateAsync(user);
                await _userManager.UpdateAsync(userToUnfollow);
            }

            return RedirectToAction("PublicProfile", new { userId = userToUnfollow.Id });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> FollowList(string userId, string listType)
        {
            var user = await _userManager.Users
                .Include(u => u.Followers)
                .Include(u => u.Following)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            IEnumerable<User> usersList;

            if (listType == "followers")
            {
                usersList = user.Followers;
            }
            else if (listType == "following")
            {
                usersList = user.Following;
            }
            else
            {
                return BadRequest("Invalid list type");
            }

            var model = new FollowListViewModel
            {
                Username = user.UserName,
                UserId = user.Id,
                ListType = listType,
                Users = usersList
            };
            return View("FollowList", model);
        }

    }
}
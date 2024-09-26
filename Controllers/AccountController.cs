using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


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
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null) {
            if (ModelState.IsValid) {
                var res = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
                
                if (res.Succeeded) {
                    if (Url.IsLocalUrl(returnUrl)) {
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
        public async Task<IActionResult> PublicProfile(string userId) {

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) {
                return NotFound("User not found");
            }

            var profileViewModel = new PublicProfileViewModel {
                Username = user.UserName,
                bio = user.Bio,
                ProfilePictureUrl = user.ProfilePicture,

                FollowerCount = user.Followers.Count,
                FollowingCount = user.Following.Count,

                Blogs = _blogRepository.GetAllBlogsByAuthor(userId)
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
                    Console.WriteLine("YHALLLLLLLLLLLLLLLLOOOOOWWWOWOWIEDFSA");
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
    }
}
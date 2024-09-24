using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace oblig1.Controllers
{
    public class AccountController : Controller {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
    }
}
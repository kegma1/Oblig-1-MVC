using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using oblig1.Models;
using oblig1.Data;

namespace oblig1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogRepository _blogRepository;

        public HomeController(ILogger<HomeController> logger, IBlogRepository blogRepository)
        {
            _logger = logger;
            _blogRepository = blogRepository;
        }

        [Authorize]
        public IActionResult Index()
        {
            var blogs = _blogRepository.GetAllBlogs();
            
            var viewModel = new IndexViewModel
            {
                Blogs = blogs
            };
            
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            return RedirectToAction("NewHome");
        }

        public IActionResult NoDbLogin()
        {
            return RedirectToAction("NewHome");
        }

        public IActionResult NewHome()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

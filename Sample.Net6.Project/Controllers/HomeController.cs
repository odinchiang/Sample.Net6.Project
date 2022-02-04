using Microsoft.AspNetCore.Mvc;

using Sample.Net6.Project.Models;

using System.Diagnostics;

namespace Sample.Net6.Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.User1 = "Odin";
            ViewData["User2"] = "Mark";
            TempData["User3"] = "Mary";
            HttpContext.Session.SetString("User4", "John");
            object User5 = "Cristina";

            return View(User5);
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
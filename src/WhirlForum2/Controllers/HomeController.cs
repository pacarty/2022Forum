using Microsoft.AspNetCore.Mvc;
using WhirlForum2.Services;

namespace WhirlForum2.Controllers
{
    public class HomeController : Controller
    {
        private IForumService _forumService;

        public HomeController(IForumService forumService)
        {
            _forumService = forumService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<string> Seed()
        {
            await _forumService.SeedDb();

            return "Database seeded";
        }
    }
}

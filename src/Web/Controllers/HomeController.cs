using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
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
            if (await _forumService.CheckDb())
            {
                return "Database seeded";
            }
            else
            {
                return "Db already seeded";
            } 
        }
    }
}

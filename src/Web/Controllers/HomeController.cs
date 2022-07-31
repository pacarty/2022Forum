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
            await _forumService.CheckDb();
            return View();
        }
    }
}

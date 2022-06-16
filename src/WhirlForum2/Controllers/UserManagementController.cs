using Microsoft.AspNetCore.Mvc;
using WhirlForum2.Services;

namespace WhirlForum2.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly int _usersOnPage = 2;
        private IForumService _forumService;

        public UserManagementController(IForumService forumService)
        {
            _forumService = forumService;
        }

        public async Task<IActionResult> Index(int? page)
        {
            return View(await _forumService.GetUserManagementModel(page ?? 1, _usersOnPage));
        }
    }
}

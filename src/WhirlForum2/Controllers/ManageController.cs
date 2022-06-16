using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhirlForum2.Entities;
using WhirlForum2.Models;

namespace WhirlForum2.Controllers
{
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ManageController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> MyAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            // if user == null throw error

            var model = new UserModel
            {
                UserId = user.Id,
                Username = user.UserName
            };

            return View(model);
        }
    }
}

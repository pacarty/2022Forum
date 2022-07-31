using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    public class UserManagementController : Controller
    {
        private IUserManagementService _userManagementService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;
        private IAuthService _authService;

        public UserManagementController(IUserManagementService userManagementService,
                                        UserManager<ApplicationUser> userManager,
                                        IAuthorizationService authorizationService,
                                        IAuthService authService)
        {
            _userManagementService = userManagementService;
            _userManager = userManager;
            _authorizationService = authorizationService;
            _authService = authService;
        }

        public async Task<IActionResult> Index(int? page)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            return View(await _userManagementService.GetUserManagementModel(page ?? 1, currentUser));
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            return View(await _userManagementService.GetUser(userId, currentUser));
        }

        [HttpPost]
        public async Task<IActionResult> EditUserRoles(EditUserModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var authResult = await _authService.CanEditUserRole(model, currentUser);

            if (!authResult)
            {
                if (User.Identity.IsAuthenticated)
                {
                    return new ForbidResult();
                }
                else
                {
                    return new ChallengeResult();
                }
            }

            await _userManagementService.EditUserRoles(model);

            return RedirectToAction("Index");
        }
    }
}

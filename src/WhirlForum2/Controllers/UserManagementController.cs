using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhirlForum2.Entities;
using WhirlForum2.Models;
using WhirlForum2.Services;

namespace WhirlForum2.Controllers
{
    [Authorize(Roles = "Root, Admin, Moderator")]
    public class UserManagementController : Controller
    {
        private readonly int _usersOnPage = 2;
        private IForumService _forumService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public UserManagementController(IForumService forumService,
                                        UserManager<ApplicationUser> userManager,
                                        IAuthorizationService authorizationService)
        {
            _forumService = forumService;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        public async Task<IActionResult> Index(int? page)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            return View(await _forumService.GetUserManagementModel(page ?? 1, _usersOnPage, currentUser));
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            return View(await _forumService.GetUser(userId, currentUser));
        }

        [Authorize(Roles = "Root, Admin")]
        [HttpPost]
        public async Task<IActionResult> EditUserRoles(EditUserModel model)
        {
            var editUser = await _userManager.FindByIdAsync(model.UserId);

            RolesAccessModel rolesAccessModel = new RolesAccessModel();
            rolesAccessModel.EditUserRoles = await _userManager.GetRolesAsync(editUser);
            rolesAccessModel.RolesToEdit = model.Roles;

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, rolesAccessModel, "AccessRolesPolicy");

            if (!authorizationResult.Succeeded)
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

            await _forumService.EditUserRoles(model);

            // return RedirectToAction("EditUser", new { userId = model.UserId });
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditUserAccess(EditUserModel model)
        {
            var editUser = await _userManager.FindByIdAsync(model.UserId);

            UserAccesModel userAccessModel = new UserAccesModel();
            userAccessModel.EditUserRoles = await _userManager.GetRolesAsync(editUser);
            userAccessModel.SubforumAccess = model.SubforumAccess;

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, userAccessModel, "AccessUserPolicy");

            if (!authorizationResult.Succeeded)
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

            await _forumService.EditUserAccess(model);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditUserModeration(EditUserModel model)
        {
            var editUser = await _userManager.FindByIdAsync(model.UserId);
            var editUserRoles = await _userManager.GetRolesAsync(editUser);

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, editUserRoles, "AccessModeratorPolicy");

            if (!authorizationResult.Succeeded)
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

            await _forumService.EditUserModeration(model);

            return RedirectToAction("Index");
        }
    }
}

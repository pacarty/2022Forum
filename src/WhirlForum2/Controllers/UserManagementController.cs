﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhirlForum2.Entities;
using WhirlForum2.Models;
using WhirlForum2.Services;

namespace WhirlForum2.Controllers
{
    [Authorize(Roles = "Root, Admin")]
    public class UserManagementController : Controller
    {
        private readonly int _usersOnPage = 2;
        private IForumService _forumService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserManagementController(IForumService forumService,
                                        UserManager<ApplicationUser> userManager)
        {
            _forumService = forumService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? page)
        {
            return View(await _forumService.GetUserManagementModel(page ?? 1, _usersOnPage));
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string userId)
        {
            return View(await _forumService.GetUser(userId));
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserModel model)
        {
            await _forumService.EditUser(model);

            // return RedirectToAction("EditUser", new { userId = model.UserId });
            return RedirectToAction("Index");
        }
    }
}

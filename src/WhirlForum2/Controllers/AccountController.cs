using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhirlForum2.Entities;
using WhirlForum2.Models;
using WhirlForum2.Services;

namespace WhirlForum2.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private IForumService _forumService;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IForumService forumService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _forumService = forumService;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            ViewBag.rootExists = false;

            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, "Root"))
                {
                    ViewBag.rootExists = true;
                    break;
                }
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Username, City = "Melbourne" };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    AddErrors(result);
                    return View(model);
                }

                result = await _userManager.AddToRoleAsync(user, "User");

                if (!result.Succeeded)
                {
                    AddErrors(result);
                    return View(model);
                }

                await _forumService.AddInitialUserClaims(user);

                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            return RedirectToAction("Index", "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        [HttpGet]
        public IActionResult SignIn(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(LoginModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ViewData["ReturnUrl"] = returnUrl;

            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RegisterRootUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterRootUser(RegisterModel model)
        {
            // final check to make sure no root users exist
            foreach (var user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, "Root"))
                {
                    // TODO: error: a root user already exists
                    return View(model);
                }
            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Username, City = "Melbourne" };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    AddErrors(result);
                    return View(model);
                }

                result = await _userManager.AddToRoleAsync(user, "Root");

                if (!result.Succeeded)
                {
                    AddErrors(result);
                    return View(model);
                }

                await _forumService.AddInitialUserClaims(user);

                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}

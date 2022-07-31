using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Authorize]
    public class ContentManagementController : Controller
    {
        private IPostManagementService _postManagementService;
        private ICommentManagementService _commentManagementService;
        private IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ContentManagementController(
                                            IPostManagementService postManagementService,
                                            ICommentManagementService commentManagementService,
                                            IAuthService authService,
                                            UserManager<ApplicationUser> userManager)
        {
            _postManagementService = postManagementService;
            _commentManagementService = commentManagementService;
            _authService = authService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> PostManagement(int? page)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return View(await _postManagementService.GetPostManagementModel(page ?? 1, currentUser));
        }

        [HttpGet]
        public async Task<IActionResult> CommentManagement(int? id, int? page)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return View(await _commentManagementService.GetCommentManagementModel(id, page ?? 1, currentUser));
        }

        [HttpPost]
        public async Task<IActionResult> DeletePostManagement(EditPostModel editPostModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var authResult = await _authService.CanDeletePostManagement(editPostModel, currentUser);

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

            await _postManagementService.DeletePostManagement(editPostModel);

            if (editPostModel.CurrentPage == 1)
            {
                return RedirectToAction("PostManagement", "ContentManagement");
            }
            else
            {
                return RedirectToAction("PostManagement", "ContentManagement", new { page = editPostModel.CurrentPage });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCommentManagement(EditCommentModel editCommentModel)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var authResult = await _authService.CanDeleteCommentManagement(editCommentModel, currentUser);

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

            await _commentManagementService.DeleteCommentManagement(editCommentModel);

            int? postId;

            if (editCommentModel.PostId == 0)
            {
                postId = null;
            }
            else
            {
                postId = editCommentModel.PostId;
            }

            if (editCommentModel.CurrentPage == 1)
            {
                return RedirectToAction("CommentManagement", "ContentManagement", new { id = postId });
            }
            else
            {
                return RedirectToAction("CommentManagement", "ContentManagement", new { id = postId, page = editCommentModel.CurrentPage });
            }
        }
    }
}

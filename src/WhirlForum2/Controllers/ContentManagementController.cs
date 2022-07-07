using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhirlForum2.Entities;
using WhirlForum2.Models;
using WhirlForum2.Services;

namespace WhirlForum2.Controllers
{
    [Authorize(Roles = "Root, Admin, Moderator")]
    public class ContentManagementController : Controller
    {
        private readonly int _postsOnPage = 2;
        private readonly int _commentsOnPage = 2;
        private IForumService _forumService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public ContentManagementController(IForumService forumService,
                                        UserManager<ApplicationUser> userManager,
                                        IAuthorizationService authorizationService)
        {
            _forumService = forumService;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        public async Task<IActionResult> PostManagement(int? page)
        {
            return View(await _forumService.GetPostManagementModel(page ?? 1, _postsOnPage));
        }

        public async Task<IActionResult> CommentManagement(int? id, int? page)
        {
                return View(await _forumService.GetCommentManagementModel(id, page ?? 1, _commentsOnPage));
            
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCommentManagement(EditCommentModel editCommentModel)
        {
            var editUser = await _forumService.GetCommentUser(editCommentModel.CommentId);

            PostAndCommentAccessModel postAndCommentAccessModel = new PostAndCommentAccessModel();
            postAndCommentAccessModel.EditUserRoles = await _userManager.GetRolesAsync(editUser);
            postAndCommentAccessModel.SubforumId = await _forumService.GetCommentSubforumId(editCommentModel.CommentId);

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, postAndCommentAccessModel, "PostAndCommentAccessPolicy");

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

            await _forumService.DeleteCommentManagement(editCommentModel);

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

        [HttpPost]
        public async Task<IActionResult> DeletePostManagement(EditPostModel editPostModel)
        {
            var editUser = await _forumService.GetPostUser(editPostModel.PostId);

            PostAndCommentAccessModel postAndCommentAccessModel = new PostAndCommentAccessModel();
            postAndCommentAccessModel.EditUserRoles = await _userManager.GetRolesAsync(editUser);
            postAndCommentAccessModel.SubforumId = await _forumService.GetPostSubforumId(editPostModel.PostId);

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, postAndCommentAccessModel, "PostAndCommentAccessPolicy");

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

            await _forumService.DeletePostManagement(editPostModel);

            if (editPostModel.CurrentPage == 1)
            {
                return RedirectToAction("PostManagement", "ContentManagement");
            }
            else
            {
                return RedirectToAction("PostManagement", "ContentManagement", new { page = editPostModel.CurrentPage });
            }
        }
    }
}

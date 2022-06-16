using Microsoft.AspNetCore.Mvc;
using WhirlForum2.Models;
using WhirlForum2.Services;

namespace WhirlForum2.Controllers
{
    public class ContentManagementController : Controller
    {
        private readonly int _postsOnPage = 2;
        private readonly int _commentsOnPage = 2;
        private IForumService _forumService;

        public ContentManagementController(IForumService forumService)
        {
            _forumService = forumService;
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

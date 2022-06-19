using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhirlForum2.Entities;
using WhirlForum2.Models;
using WhirlForum2.Services;

namespace WhirlForum2.Controllers
{
    public class ForumController : Controller
    {
        private readonly int _postsOnPage = 2;
        private readonly int _commentsOnPage = 2;
        private readonly int _postsToDisplay = 3;
        private readonly int _commentsToDisplay = 3;
        private IForumService _forumService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAuthorizationService _authorizationService;

        public ForumController(IForumService forumService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IAuthorizationService authorizationService)
        {
            _forumService = forumService;
            _userManager = userManager;
            _signInManager = signInManager;
            _authorizationService = authorizationService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _forumService.GetSubforumModels(_postsToDisplay));
        }

        public async Task<IActionResult> Topic(int id, int? page)
        {
            return View(await _forumService.GetTopicModel(id, page ?? 1, _postsOnPage, _commentsToDisplay));
        }

        public async Task<IActionResult> Post(int id, int? page)
        {
            string? currentUserId = null;

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                currentUserId = null;
            }
            else
            {
                currentUserId = user.Id;
            }

            return View(await _forumService.GetPostModel(id, page ?? 1, _commentsOnPage, currentUserId));
        }

        [HttpGet]
        [Authorize]
        public IActionResult NewPost(int id)
        {
            return View(new NewPostModel { TopicId = id });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> NewPost(NewPostModel newPostModel)
        {
            var user = await _userManager.GetUserAsync(User);
            // if user == null throw error

            newPostModel.UserId = user.Id;

            await _forumService.AddPost(newPostModel);

            return RedirectToAction("Topic", "Forum", new { id = newPostModel.TopicId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> NewComment(NewCommentModel newCommentModel)
        {
            var user = await _userManager.GetUserAsync(User);
            // if user == null throw error

            newCommentModel.UserId = user.Id;

            await _forumService.AddComment(newCommentModel);

            if (newCommentModel.CurrentPage == 1)
            {
                return RedirectToAction("Post", "Forum", new { id = newCommentModel.PostId });
            } else
            {
                return RedirectToAction("Post", "Forum", new { id = newCommentModel.PostId, page = newCommentModel.CurrentPage });
            }   
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> EditComment(EditCommentModel editCommentModel, string content)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, editCommentModel.UserId, "UserEditDeletePolicy");
            
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
            
            editCommentModel.Content = content;

            await _forumService.EditComment(editCommentModel);

            if (editCommentModel.CurrentPage == 1)
            {
                return RedirectToAction("Post", "Forum", new { id = editCommentModel.PostId });
            }
            else
            {
                return RedirectToAction("Post", "Forum", new { id = editCommentModel.PostId, page = editCommentModel.CurrentPage });
            }
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> DeleteComment(EditCommentModel editCommentModel)
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, editCommentModel.UserId, "UserEditDeletePolicy");

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

            await _forumService.DeleteComment(editCommentModel);

            if (editCommentModel.CurrentPage == 1)
            {
                return RedirectToAction("Post", "Forum", new { id = editCommentModel.PostId });
            }
            else
            {
                return RedirectToAction("Post", "Forum", new { id = editCommentModel.PostId, page = editCommentModel.CurrentPage });
            }
        }
    }
}

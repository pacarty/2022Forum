using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Web.Controllers
{
    public class ForumController : Controller
    {
        private IForumService _forumService;
        private ISubforumService _subforumService;
        private ITopicService _topicService;
        private IPostService _postService;
        private ICommentService _commentService;
        private IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ForumController(
            IForumService forumService,
            ISubforumService subforumService,
            ITopicService topicService,
            IPostService postService,
            ICommentService commentService,
            IAuthService authService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _forumService = forumService;
            _subforumService = subforumService;
            _topicService = topicService;
            _postService = postService;
            _commentService = commentService;
            _authService = authService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _subforumService.GetSubforumModels());
        }

        [HttpGet]
        public async Task<IActionResult> Topic(int id, int? page)
        {
            return View(await _topicService.GetTopicModel(id, page ?? 1));
        }

        [HttpGet]
        public async Task<IActionResult> Post(int id, int? page)
        {
            Claim? currentUserClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            string? currentUserId = currentUserClaim?.Value;

            return View(await _postService.GetPostModel(id, page ?? 1, currentUserId));
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
            // var user = await _userManager.GetUserAsync(User);
            // if user == null throw error

            newPostModel.UserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            await _postService.AddPost(newPostModel);

            return RedirectToAction("Topic", "Forum", new { id = newPostModel.TopicId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> NewComment(NewCommentModel newCommentModel)
        {
            // var user = await _userManager.GetUserAsync(User);
            // if user == null throw error

            newCommentModel.UserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            await _commentService.AddComment(newCommentModel);

            if (newCommentModel.CurrentPage == 1)
            {
                return RedirectToAction("Post", "Forum", new { id = newCommentModel.PostId });
            }
            else
            {
                return RedirectToAction("Post", "Forum", new { id = newCommentModel.PostId, page = newCommentModel.CurrentPage });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditComment(EditCommentModel editCommentModel, string content)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var authResult = await _authService.CanEditOrDeleteComment(editCommentModel.UserId, currentUser);

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

            editCommentModel.Content = content;

            await _commentService.EditComment(editCommentModel);

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
        [Authorize]
        public async Task<IActionResult> DeleteComment(EditCommentModel editCommentModel, string content)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var authResult = await _authService.CanEditOrDeleteComment(editCommentModel.UserId, currentUser);

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

            await _commentService.DeleteComment(editCommentModel);

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

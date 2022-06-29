using WhirlForum2.Entities;
using WhirlForum2.Models;

namespace WhirlForum2.Services
{
    public interface IForumService
    {
        Task SeedDb();
        Task<List<SubforumModel>> GetSubforumModels(int postsToDisplay);
        Task<TopicModel> GetTopicModel(int topicId, int pageIndex, int postsOnPage, int commentsToDisplay);
        Task<PostModel> GetPostModel(int postId, int pageIndex, int commentsOnPage, string? currentUserId);
        Task AddPost(NewPostModel newPostModel);
        Task AddComment(NewCommentModel newCommentModel);
        Task<PostManagementModel> GetPostManagementModel(int pageIndex, int postsOnPage);
        Task<CommentManagementModel> GetCommentManagementModel(int? postId, int pageIndex, int commentsOnPage);
        Task<UserManagementModel> GetUserManagementModel(int pageIndex, int usersOnPage, ApplicationUser currentUser);
        Task EditComment(EditCommentModel editCommentModel);
        Task DeleteComment(EditCommentModel editCommentModel);
        Task DeleteCommentManagement(EditCommentModel editCommentModel);
        Task DeletePostManagement(EditPostModel editPostModel);
        Task<EditUserModel> GetUser(string userId, ApplicationUser currentUser);
        Task EditUserRoles(EditUserModel editUserModel);
        Task AddInitialUserClaims(ApplicationUser user);
        Task EditUserAccess(EditUserModel editUserModel);
        Task EditUserModeration(EditUserModel editUserModel);
    }
}

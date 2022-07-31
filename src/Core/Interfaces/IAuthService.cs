using Core.Entities;
using Core.Models;

namespace Core.Interfaces
{
    public interface IAuthService
    {
        Task<bool> DoesRootExist();
        Task<bool> CanEditUserRole(EditUserModel model, ApplicationUser currentUser);
        Task<bool> CanEditOrDeleteComment(string commentUserId, ApplicationUser currentUser);
        Task<bool> CanDeletePostManagement(EditPostModel editPostModel, ApplicationUser currentUser);
        Task<bool> CanDeleteCommentManagement(EditCommentModel editCommentModel, ApplicationUser currentUser);
    }
}

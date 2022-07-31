using Core.Entities;
using Core.Models;

namespace Core.Interfaces
{
    public interface ICommentManagementService
    {
        Task<CommentManagementModel> GetCommentManagementModel(int? postId, int pageIndex, ApplicationUser currentUser);
        Task DeleteCommentManagement(EditCommentModel editCommentModel);
    }
}

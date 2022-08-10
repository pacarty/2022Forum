using Core.Entities;
using Core.Models;

namespace Core.Interfaces
{
    public interface ICommentService
    {
        Task AddComment(NewCommentModel newCommentModel, ApplicationUser currentUser);
        Task EditComment(EditCommentModel editCommentModel);
        Task DeleteComment(EditCommentModel editCommentModel);
    }
}

using Core.Models;

namespace Core.Interfaces
{
    public interface ICommentService
    {
        Task AddComment(NewCommentModel newCommentModel);
        Task EditComment(EditCommentModel editCommentModel);
        Task DeleteComment(EditCommentModel editCommentModel);
    }
}

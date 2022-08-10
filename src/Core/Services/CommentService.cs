using Core.Data;
using Core.Entities;
using Core.Interfaces;
using Core.Models;

namespace Core.Services
{
    public class CommentService : ICommentService
    {
        private readonly DataContext _context;

        public CommentService(DataContext context)
        {
            _context = context;
        }

        public async Task AddComment(NewCommentModel newCommentModel, ApplicationUser currentUser)
        {
            await _context.Comments.AddAsync(
                new Comment
                {
                    PostId = newCommentModel.PostId,
                    Content = newCommentModel.Content,
                    UserId = currentUser.Id
                });

            await _context.SaveChangesAsync();
        }

        public async Task EditComment(EditCommentModel editCommentModel)
        {
            Comment comment = await _context.Comments.FindAsync(editCommentModel.CommentId);
            comment.Content = editCommentModel.Content;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteComment(EditCommentModel editCommentModel)
        {
            Comment comment = await _context.Comments.FindAsync(editCommentModel.CommentId);
            _context.Remove(comment);

            await _context.SaveChangesAsync();
        }
    }
}

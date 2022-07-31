using Core.Constants;
using Core.Data;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
    public class CommentManagementService : ICommentManagementService
    {
        private readonly DataContext _context;

        public CommentManagementService(DataContext context)
        {
            _context = context;
        }

        private async Task<bool> AuthToDelete(Comment comment, ApplicationUser currentUser)
        {
            var commentUser = await _context.Users.FindAsync(comment.UserId);

            if (currentUser.AuthAccessLevel == 4)
            {
                return true;
            }

            if (currentUser.AuthAccessLevel > commentUser.AuthAccessLevel)
            {
                return true;
            }

            return false;
        }

        private async Task<List<CommentModel>> GetCommentModelsOnPage(List<Comment> comments, ApplicationUser currentUser)
        {
            List<CommentModel> commentModels = new List<CommentModel>();

            foreach (Comment comment in comments)
            {
                commentModels.Add(new CommentModel
                {
                    Comment = comment,
                    CommentUser = await _context.Users.FindAsync(comment.UserId),
                    DisplayEditDelete = await AuthToDelete(comment, currentUser)
                });
            }

            return commentModels;
        }

        private PaginationInfo GetCommentManagementPaginationInfo(int totalItems, int pageIndex)
        {
            return new PaginationInfo
            {
                TotalItems = totalItems,
                ItemsPerPage = PaginationConstants.COMMENTS_ON_PAGE,
                CurrentPage = pageIndex,
                TotalPages = int.Parse(Math.Ceiling(((decimal)totalItems / PaginationConstants.COMMENTS_ON_PAGE)).ToString()),
                Previous = pageIndex - 1,
                Next = pageIndex + 1
            };
        }

        public async Task<CommentManagementModel> GetCommentManagementModel(int? postId, int pageIndex, ApplicationUser currentUser)
        {
            // need to initialise these for some reason
            List<Comment> comments = new List<Comment>();
            int totalItems = 0;

            if (postId == null)
            {
                comments = await _context.Comments
                .Skip(PaginationConstants.COMMENTS_ON_PAGE * (pageIndex - 1))
                .Take(PaginationConstants.COMMENTS_ON_PAGE)
                .ToListAsync();

                totalItems = await _context.Comments.CountAsync();

                if (comments.Count == 0)
                {
                    pageIndex = 1;

                    comments = await _context.Comments
                        .Skip(PaginationConstants.COMMENTS_ON_PAGE * (pageIndex - 1))
                        .Take(PaginationConstants.COMMENTS_ON_PAGE)
                        .ToListAsync();

                    totalItems = await _context.Comments.CountAsync();
                }
            }
            else
            {
                comments = await _context.Comments.Where(c => c.PostId == postId)
                .Skip(PaginationConstants.COMMENTS_ON_PAGE * (pageIndex - 1))
                .Take(PaginationConstants.COMMENTS_ON_PAGE)
                .ToListAsync();

                totalItems = await _context.Comments.Where(c => c.PostId == postId).CountAsync();

                if (comments.Count == 0)
                {
                    pageIndex = 1;

                    comments = await _context.Comments.Where(c => c.PostId == postId)
                        .Skip(PaginationConstants.COMMENTS_ON_PAGE * (pageIndex - 1))
                        .Take(PaginationConstants.COMMENTS_ON_PAGE)
                        .ToListAsync();

                    totalItems = await _context.Comments.Where(c => c.PostId == postId).CountAsync();
                }
            }

            EditCommentModel editCommentModel = new EditCommentModel { CurrentPage = pageIndex };

            if (postId != null)
            {
                editCommentModel.PostId = (int)postId;
            }

            return new CommentManagementModel
            {
                CommentModels = await GetCommentModelsOnPage(comments, currentUser),
                PaginationInfo = GetCommentManagementPaginationInfo(totalItems, pageIndex),
                EditCommentModel = editCommentModel
            };
        }

        public async Task DeleteCommentManagement(EditCommentModel editCommentModel)
        {
            Comment comment = await _context.Comments.FindAsync(editCommentModel.CommentId);
            _context.Remove(comment);

            await _context.SaveChangesAsync();
        }
    }
}

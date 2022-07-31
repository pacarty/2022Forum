using Core.Data;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;

        public AuthService(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> DoesRootExist()
        {
            if (await _context.Users.Where(u => u.AuthAccessLevel == 4).CountAsync() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private int GetAuthAccessLevelFromRole(string role)
        {
            int authAccessLevel = 0;

            if (role == "Root")
            {
                authAccessLevel = 4;
            }
            else if (role == "Admin")
            {
                authAccessLevel = 3;
            }
            else if (role == "Moderator")
            {
                authAccessLevel = 2;
            }
            else if (role == "User")
            {
                authAccessLevel = 1;
            }
            else
            {
                authAccessLevel = 0;
            }

            return authAccessLevel;
        }

        public async Task<bool> CanEditUserRole(EditUserModel model, ApplicationUser currentUser)
        {

            if (currentUser.AuthAccessLevel < 3)
            {
                return false;
            }

            if (currentUser.AuthAccessLevel == 4)
            {
                return true;
            }

            var userToEdit = await _context.Users.FindAsync(model.User.Id);

            if (currentUser.AuthAccessLevel > userToEdit.AuthAccessLevel)
            {
                if (currentUser.AuthAccessLevel > GetAuthAccessLevelFromRole(model.Role))
                {
                    return true;
                }

                return false;
            }

            return false;
        }

        public async Task<bool> CanEditOrDeleteComment(string commentUserId, ApplicationUser currentUser)
        {
            return commentUserId == currentUser.Id;
        }

        public async Task<bool> CanDeletePostManagement(EditPostModel editPostModel, ApplicationUser currentUser)
        {
            if (currentUser.AuthAccessLevel < 3)
            {
                return false;
            }

            if (currentUser.AuthAccessLevel == 4)
            {
                return true;
            }

            Post postToEdit = await _context.Posts.FirstOrDefaultAsync(p => p.Id == editPostModel.PostId);
            var userToEdit = await _context.Users.FindAsync(postToEdit.UserId);

            if (currentUser.AuthAccessLevel > userToEdit.AuthAccessLevel)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> CanDeleteCommentManagement(EditCommentModel editCommentModel, ApplicationUser currentUser)
        {
            if (currentUser.AuthAccessLevel < 3)
            {
                return false;
            }

            if (currentUser.AuthAccessLevel == 4)
            {
                return true;
            }

            Comment commentToEdit = await _context.Comments.FirstOrDefaultAsync(c => c.Id == editCommentModel.CommentId);
            var userToEdit = await _context.Users.FindAsync(commentToEdit.UserId);

            if (currentUser.AuthAccessLevel > userToEdit.AuthAccessLevel)
            {
                return true;
            }

            return false;
        }
    }
}

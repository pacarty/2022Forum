using Core.Constants;
using Core.Data;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly DataContext _context;

        public UserManagementService(DataContext context)
        {
            _context = context;
        }

        private PaginationInfo GetUserManagementPaginationInfo(int totalItems, int pageIndex)
        {
            return new PaginationInfo
            {
                TotalItems = totalItems,
                ItemsPerPage = PaginationConstants.USERS_ON_PAGE,
                CurrentPage = pageIndex,
                TotalPages = int.Parse(Math.Ceiling(((decimal)totalItems / PaginationConstants.USERS_ON_PAGE)).ToString()),
                Previous = pageIndex - 1,
                Next = pageIndex + 1
            };
        }

        public async Task<UserManagementModel> GetUserManagementModel(int pageIndex, ApplicationUser currentUser)
        {
            List<ApplicationUser> users = new List<ApplicationUser>();
            int totalItems = 0;

            if (currentUser.AuthAccessLevel == 4)
            {
                users = await _context.Users
                    .Skip(PaginationConstants.USERS_ON_PAGE * (pageIndex - 1))
                    .Take(PaginationConstants.USERS_ON_PAGE)
                    .ToListAsync();

                totalItems = await _context.Users.CountAsync();
            }
            else if (currentUser.AuthAccessLevel == 3)
            {
                users = await _context.Users.Where(u => u.AuthAccessLevel < 3)
                    .Skip(PaginationConstants.USERS_ON_PAGE * (pageIndex - 1))
                    .Take(PaginationConstants.USERS_ON_PAGE)
                    .ToListAsync();

                totalItems = await _context.Users.Where(u => u.AuthAccessLevel < 3).CountAsync();
            }
            else if (currentUser.AuthAccessLevel == 2)
            {
                users = await _context.Users.Where(u => u.AuthAccessLevel < 2)
                    .Skip(PaginationConstants.USERS_ON_PAGE * (pageIndex - 1))
                    .Take(PaginationConstants.USERS_ON_PAGE)
                    .ToListAsync();

                totalItems = await _context.Users.Where(u => u.AuthAccessLevel < 2).CountAsync();
            }

            UserManagementModel userManagementModel = new UserManagementModel
            {
                Users = users,
                PaginationInfo = GetUserManagementPaginationInfo(totalItems, pageIndex)
            };

            return userManagementModel;
        }

        private string GetRoleFromAccessLevel(int accessLevel)
        {
            if (accessLevel == 4)
            {
                return "Root";
            }
            else if (accessLevel == 3)
            {
                return "Admin";
            }
            else if (accessLevel == 2)
            {
                return "Moderator";
            }
            else if (accessLevel == 1)
            {
                return "User";
            }
            else
            {
                return "Banned";
            }
        }

        private int GetAccessLevelFromRole(string role)
        {
            if (role == "Root")
            {
                return 4;
            }
            else if (role == "Admin")
            {
                return 3;
            }
            else if (role == "Moderator")
            {
                return 2;
            }
            else if (role == "User")
            {
                return 1;
            }
            else if (role == "Banned")
            {
                return 0;
            }
            else
            {
                return 0;
            }
        }

        public async Task<EditUserModel> GetUser(string userId, ApplicationUser currentUser)
        {
            var userToEdit = await _context.Users.FindAsync(userId);
            var userRole = GetRoleFromAccessLevel(userToEdit.AuthAccessLevel);

            EditUserModel editUserModel = new EditUserModel
            {
                User = userToEdit,
                Role = userRole
            };

            if (currentUser.AuthAccessLevel == 4)
            {
                editUserModel.Roles = new List<string> { "Root", "Admin", "User", "Banned" };
                return editUserModel;
            }
            else if (currentUser.AuthAccessLevel == 3)
            {
                if (userToEdit.AuthAccessLevel < 3)
                {
                    editUserModel.Roles = new List<string> { "User", "Banned" };
                    return editUserModel;
                }
            }
            else
            {
                return null;
            }

            return null;
        }

        public async Task EditUserRoles(EditUserModel model)
        {
            var userToEdit = await _context.Users.FindAsync(model.User.Id);
            userToEdit.AuthAccessLevel = GetAccessLevelFromRole(model.Role);

            await _context.SaveChangesAsync();
        }
    }
}

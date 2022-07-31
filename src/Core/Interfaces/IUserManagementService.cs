using Core.Entities;
using Core.Models;

namespace Core.Interfaces
{
    public interface IUserManagementService
    {
        Task<UserManagementModel> GetUserManagementModel(int pageIndex, ApplicationUser currentUser);
        Task<EditUserModel> GetUser(string userId, ApplicationUser currentUser);
        Task EditUserRoles(EditUserModel model);
    }
}

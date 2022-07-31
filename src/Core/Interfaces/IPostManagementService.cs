using Core.Entities;
using Core.Models;

namespace Core.Interfaces
{
    public interface IPostManagementService
    {
        Task<PostManagementModel> GetPostManagementModel(int pageIndex, ApplicationUser currentUser);
        Task DeletePostManagement(EditPostModel editPostModel);
    }
}

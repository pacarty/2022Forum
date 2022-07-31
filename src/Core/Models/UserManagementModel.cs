using Core.Entities;

namespace Core.Models
{
    public class UserManagementModel
    {
        public List<ApplicationUser> Users { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
    }
}

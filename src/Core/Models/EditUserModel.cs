using Core.Entities;

namespace Core.Models
{
    public class EditUserModel
    {
        public ApplicationUser User { get; set; }
        public List<string> Roles { get; set; }
        public string Role { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;

namespace Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public int AuthAccessLevel { get; set; }
    }
}

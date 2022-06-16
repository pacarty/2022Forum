using Microsoft.AspNetCore.Identity;

namespace WhirlForum2.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string City { get; set; }
    }
}

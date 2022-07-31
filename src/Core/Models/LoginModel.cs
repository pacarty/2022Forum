using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}

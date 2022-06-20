namespace WhirlForum2.Models
{
    public class EditUserModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<UserRolesModel> Roles { get; set; } = new List<UserRolesModel>();
    }
}

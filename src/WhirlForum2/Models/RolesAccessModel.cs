namespace WhirlForum2.Models
{
    public class RolesAccessModel
    {
        public IList<string> EditUserRoles { get; set; }
        public List<UserRolesModel> RolesToEdit { get; set; }
    }
}

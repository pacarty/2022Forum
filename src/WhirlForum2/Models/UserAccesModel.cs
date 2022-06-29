namespace WhirlForum2.Models
{
    public class UserAccesModel
    {
        public IList<string> EditUserRoles { get; set; }
        public List<SubforumAccess> SubforumAccess { get; set; } = new List<SubforumAccess>();
    }
}

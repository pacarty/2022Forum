namespace WhirlForum2.Models
{
    public class SubforumAccess
    {
        public int SubforumId { get; set; }
        public string SubforumName { get; set; }
        public bool UserAccess { get; set; }
        public bool ModAccess { get; set; }
    }
}

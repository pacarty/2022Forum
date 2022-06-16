namespace WhirlForum2.Models
{
    public class CommentModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public UserModel UserModel { get; set; }
        public int PostId { get; set; }
        public string PostName { get; set; }
        public bool IsUser { get; set; }
    }
}

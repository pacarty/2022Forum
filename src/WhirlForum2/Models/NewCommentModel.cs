namespace WhirlForum2.Models
{
    public class NewCommentModel
    {
        public int PostId { get; set; }
        public string Content { get; set; }
        public int CurrentPage { get; set; }
        public string UserId { get; set; }
    }
}

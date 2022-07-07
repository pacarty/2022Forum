namespace WhirlForum2.Models
{
    public class PostModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CommentModel> CommentModels { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
        public NewCommentModel NewCommentModel { get; set; }
        public EditCommentModel EditCommentModel { get; set; }
        public UserModel UserModel { get; set; }
        public int SubforumId { get; set; }
    }
}

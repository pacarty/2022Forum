namespace WhirlForum2.Models
{
    public class CommentManagementModel
    {
        public List<CommentModel> CommentModels { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
        public EditCommentModel EditCommentModel { get; set; }
    }
}

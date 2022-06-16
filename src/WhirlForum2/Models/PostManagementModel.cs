namespace WhirlForum2.Models
{
    public class PostManagementModel
    {
        public List<PostModel> PostModels { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
        public EditPostModel EditPostModel { get; set; }
    }
}

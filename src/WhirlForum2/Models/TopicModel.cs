namespace WhirlForum2.Models
{
    public class TopicModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<PostModel> PostModels { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
    }
}

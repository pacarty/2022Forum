namespace WhirlForum2.Models
{
    public class SubforumModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<TopicModel> TopicModels { get; set; }
    }
}

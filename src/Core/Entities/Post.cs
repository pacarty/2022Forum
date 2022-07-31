namespace Core.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TopicId { get; set; }
        public string UserId { get; set; }
    }
}

using Core.Entities;

namespace Core.Models
{
    public class TopicModel
    {
        public Topic Topic { get; set; }
        public List<PostModel> PostModels { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
    }
}

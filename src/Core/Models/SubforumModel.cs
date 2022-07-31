using Core.Entities;

namespace Core.Models
{
    public class SubforumModel
    {
        public Subforum Subforum { get; set; }
        public List<TopicModel> TopicModels { get; set; }
    }
}

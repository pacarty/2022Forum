using Core.Models;

namespace Core.Interfaces
{
    public interface ITopicService
    {
        Task<TopicModel> GetTopicModel(int topicId, int pageIndex);
    }
}

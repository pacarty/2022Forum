using Core.Constants;
using Core.Data;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
    public class SubforumService : ISubforumService
    {
        private readonly DataContext _context;

        public SubforumService(DataContext context)
        {
            _context = context;
        }

        private async Task<List<PostModel>> GetPostModelsOnPage(Topic topic)
        {
            List<PostModel> postModels = new List<PostModel>();

            foreach (Post post in await _context.Posts.Where(p => p.TopicId == topic.Id).Take(PaginationConstants.POSTS_TO_PREVIEW).ToListAsync())
            {
                postModels.Add(new PostModel
                {
                    Post = post,
                    User = await _context.Users.FindAsync(post.UserId)
                });
            }

            return postModels;
        }

        private async Task<List<TopicModel>> GetTopicModels(Subforum subforum)
        {
            List<TopicModel> topicModels = new List<TopicModel>();

            foreach (Topic topic in await _context.Topics.Where(t => t.SubforumId == subforum.Id).ToListAsync())
            {
                List<PostModel> postModels = await GetPostModelsOnPage(topic);

                topicModels.Add(new TopicModel
                {
                    Topic = topic,
                    PostModels = postModels
                });
            }

            return topicModels;
        }

        public async Task<List<SubforumModel>> GetSubforumModels()
        {
            List<SubforumModel> subforumModels = new List<SubforumModel>();

            foreach (Subforum subforum in await _context.Subforums.ToListAsync())
            {
                List<TopicModel> topicModels = await GetTopicModels(subforum);

                subforumModels.Add(new SubforumModel
                {
                    Subforum = subforum,
                    TopicModels = topicModels
                });
            }

            return subforumModels;
        }
    }
}

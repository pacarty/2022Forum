using Core.Constants;
using Core.Data;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
    public class TopicService : ITopicService
    {
        private readonly DataContext _context;

        public TopicService(DataContext context)
        {
            _context = context;
        }

        private async Task<List<CommentModel>> GetCommentModelsInPost(Post post)
        {
            List<CommentModel> commentModels = new List<CommentModel>();

            foreach (Comment comment in await _context.Comments.Where(c => c.PostId == post.Id).Take(PaginationConstants.COMMENTS_TO_PREVIEW).ToListAsync())
            {
                commentModels.Add(new CommentModel
                {
                    Comment = comment,
                    CommentUser = await _context.Users.FindAsync(comment.UserId)

                });
            }

            return commentModels;
        }

        private async Task<List<PostModel>> GetPostModelsOnPage(List<Post> posts)
        {
            List<PostModel> postModels = new List<PostModel>();

            foreach (Post post in posts)
            {
                List<CommentModel> commentModels = await GetCommentModelsInPost(post);

                postModels.Add(new PostModel
                {
                    Post = post,
                    CommentModels = commentModels,
                    User = await _context.Users.FindAsync(post.UserId)
                });
            }

            return postModels;
        }

        private PaginationInfo GetTopicPaginationInfo(int totalItems, int pageIndex)
        {
            return new PaginationInfo
            {
                TotalItems = totalItems,
                ItemsPerPage = PaginationConstants.POSTS_ON_PAGE,
                CurrentPage = pageIndex,
                TotalPages = int.Parse(Math.Ceiling(((decimal)totalItems / PaginationConstants.POSTS_ON_PAGE)).ToString()),
                Previous = pageIndex - 1,
                Next = pageIndex + 1
            };
        }

        public async Task<TopicModel> GetTopicModel(int topicId, int pageIndex)
        {
            TopicModel topicModel = new TopicModel();
            topicModel.Topic = await _context.Topics.FindAsync(topicId);

            List<Post> posts = await _context.Posts.Where(p => p.TopicId == topicId)
                .Skip(PaginationConstants.POSTS_ON_PAGE * (pageIndex - 1))
                .Take(PaginationConstants.POSTS_ON_PAGE)
                .ToListAsync();

            int totalItems = await _context.Posts.Where(p => p.TopicId == topicId).CountAsync();
            topicModel.PostModels = await GetPostModelsOnPage(posts);
            topicModel.PaginationInfo = GetTopicPaginationInfo(totalItems, pageIndex);

            return topicModel;
        }
    }
}

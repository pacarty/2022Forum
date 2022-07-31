using Core.Constants;
using Core.Data;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
    public class PostService : IPostService
    {
        private readonly DataContext _context;

        public PostService(DataContext context)
        {
            _context = context;
        }

        private async Task<List<CommentModel>> GetCommentModelsOnPage(List<Comment> comments, string? currentUserId)
        {
            List<CommentModel> commentModels = new List<CommentModel>();

            foreach (Comment comment in comments)
            {
                var commentUser = await _context.Users.FindAsync(comment.UserId);
                bool displayEditDelete = false;

                if (currentUserId == commentUser.Id)
                {
                    displayEditDelete = true;
                }

                commentModels.Add(new CommentModel
                {
                    Comment = comment,
                    CommentUser = commentUser,
                    DisplayEditDelete = displayEditDelete
                });
            }

            return commentModels;
        }

        private PaginationInfo GetPostPaginationInfo(int totalItems, int pageIndex)
        {
            return new PaginationInfo
            {
                TotalItems = totalItems,
                ItemsPerPage = PaginationConstants.COMMENTS_ON_PAGE,
                CurrentPage = pageIndex,
                TotalPages = int.Parse(Math.Ceiling(((decimal)totalItems / PaginationConstants.COMMENTS_ON_PAGE)).ToString()),
                Previous = pageIndex - 1,
                Next = pageIndex + 1
            };
        }

        public async Task<PostModel> GetPostModel(int postId, int pageIndex, string? currentUserId)
        {
            PostModel postModel = new PostModel();

            postModel.Post = await _context.Posts.FindAsync(postId);
            postModel.User = await _context.Users.FindAsync(postModel.Post.UserId);

            List<Comment> comments = await _context.Comments.Where(c => c.PostId == postId)
                .Skip(PaginationConstants.COMMENTS_ON_PAGE * (pageIndex - 1))
                .Take(PaginationConstants.COMMENTS_ON_PAGE)
                .ToListAsync();

            if (comments.Count == 0)
            {
                pageIndex = 1;

                comments = await _context.Comments.Where(c => c.PostId == postId)
                    .Skip(PaginationConstants.COMMENTS_ON_PAGE * (pageIndex - 1))
                    .Take(PaginationConstants.COMMENTS_ON_PAGE)
                    .ToListAsync();
            }

            int totalItems = await _context.Comments.Where(c => c.PostId == postId).CountAsync();
            postModel.CommentModels = await GetCommentModelsOnPage(comments, currentUserId);
            postModel.PaginationInfo = GetPostPaginationInfo(totalItems, pageIndex);

            postModel.NewCommentModel = new NewCommentModel
            {
                PostId = postId,
                CurrentPage = pageIndex
            };

            postModel.EditCommentModel = new EditCommentModel
            {
                PostId = postId,
                CurrentPage = pageIndex
            };

            return postModel;
        }

        public async Task AddPost(NewPostModel newPostModel)
        {
            // post variable declared so we can use post.Id to set new Comment PostId to this variable's Id.
            // after we call savechangesasync() we are able to use this variables primary key (Id).
            Post post = new Post
            {
                Name = newPostModel.Title,
                TopicId = newPostModel.TopicId,
                UserId = newPostModel.UserId
            };

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            await _context.Comments.AddAsync(
                new Comment
                {
                    Content = newPostModel.Content,
                    PostId = post.Id,
                    UserId = newPostModel.UserId
                });

            await _context.SaveChangesAsync();
        }
    }
}

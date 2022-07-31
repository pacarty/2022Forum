using Core.Constants;
using Core.Data;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
    public class PostManagementService : IPostManagementService
    {
        private readonly DataContext _context;

        public PostManagementService(DataContext context)
        {
            _context = context;
        }

        private async Task<bool> AuthToDelete(Post post, ApplicationUser currentUser)
        {
            var postUser = await _context.Users.FindAsync(post.UserId);

            if (currentUser.AuthAccessLevel == 4)
            {
                return true;
            }

            if (currentUser.AuthAccessLevel > postUser.AuthAccessLevel)
            {
                return true;
            }

            return false;
        }

        private async Task<List<PostModel>> GetPostModelsOnPage(List<Post> posts, ApplicationUser currentUser)
        {
            List<PostModel> postModels = new List<PostModel>();

            foreach (Post post in posts)
            {
                postModels.Add(new PostModel
                {
                    Post = post,
                    User = await _context.Users.FindAsync(post.UserId),
                    DisplayEditDelete = await AuthToDelete(post, currentUser)
                });
            }

            return postModels;
        }

        private PaginationInfo GetPostManagementPaginationInfo(int totalItems, int pageIndex)
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

        public async Task<PostManagementModel> GetPostManagementModel(int pageIndex, ApplicationUser currentUser)
        {
            List<Post> posts = await _context.Posts
                .Skip(PaginationConstants.POSTS_ON_PAGE * (pageIndex - 1))
                .Take(PaginationConstants.POSTS_ON_PAGE)
                .ToListAsync();

            if (posts.Count == 0)
            {
                pageIndex = 1;

                posts = await _context.Posts
                    .Skip(PaginationConstants.POSTS_ON_PAGE * (pageIndex - 1))
                    .Take(PaginationConstants.POSTS_ON_PAGE)
                    .ToListAsync();
            }

            int totalItems = await _context.Posts.CountAsync();

            return new PostManagementModel
            {
                PostModels = await GetPostModelsOnPage(posts, currentUser),
                PaginationInfo = GetPostManagementPaginationInfo(totalItems, pageIndex),
                EditPostModel = new EditPostModel { CurrentPage = pageIndex }
            };
        }

        public async Task DeletePostManagement(EditPostModel editPostModel)
        {
            Post post = await _context.Posts.FindAsync(editPostModel.PostId);
            List<Comment> comments = await _context.Comments.Where(c => c.PostId == post.Id).ToListAsync();

            _context.RemoveRange(comments);
            _context.Remove(post);

            await _context.SaveChangesAsync();
        }
    }
}

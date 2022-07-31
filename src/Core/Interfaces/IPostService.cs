using Core.Models;

namespace Core.Interfaces
{
    public interface IPostService
    {
        Task<PostModel> GetPostModel(int postId, int pageIndex, string? currentUserId);
        Task AddPost(NewPostModel newPostModel);
    }
}

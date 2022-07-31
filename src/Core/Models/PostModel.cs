using Core.Entities;

namespace Core.Models
{
    public class PostModel
    {
        public Post Post { get; set; }
        public List<CommentModel> CommentModels { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
        public NewCommentModel NewCommentModel { get; set; }
        public EditCommentModel EditCommentModel { get; set; }
        public ApplicationUser User { get; set; }
        public bool DisplayEditDelete { get; set; }
    }
}

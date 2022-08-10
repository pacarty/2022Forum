using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class EditCommentModel
    {
        public int CommentId { get; set; }

        [Required]
        public string Content { get; set; }
        public int CurrentPage { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
    }
}

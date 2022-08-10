using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class NewCommentModel
    {
        public int PostId { get; set; }

        [Required]
        public string Content { get; set; }

        public int CurrentPage { get; set; }
    }
}

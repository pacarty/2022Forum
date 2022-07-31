using Core.Entities;

namespace Core.Models
{
    public class CommentModel
    {
        public Comment Comment { get; set; }
        public ApplicationUser CommentUser { get; set; }
        public bool DisplayEditDelete { get; set; }
    }
}

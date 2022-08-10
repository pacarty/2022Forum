using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class NewPostModel
    {
        public int TopicId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
    }
}

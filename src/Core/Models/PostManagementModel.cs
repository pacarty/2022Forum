using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class PostManagementModel
    {
        public List<PostModel> PostModels { get; set; }
        public PaginationInfo PaginationInfo { get; set; }
        public EditPostModel EditPostModel { get; set; }
    }
}

namespace WhirlForum2.Models
{
    public class PaginationInfo
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int Previous { get; set; }
        public int Next { get; set; }
    }
}

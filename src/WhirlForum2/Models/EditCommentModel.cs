﻿namespace WhirlForum2.Models
{
    public class EditCommentModel
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public int CurrentPage { get; set; }
        public int PostId { get; set; }
        public string CurrentUserId { get; set; }
    }
}

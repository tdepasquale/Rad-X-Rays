using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class Blog
    {
        public Guid Id { get; set; }
        public string AppUserId { get; set; }
        public string OwnerUsername { get; set; }
        public string Title { get; set; }
        public string CoverImageUrl { get; set; }
        public List<BlogSection> Sections { get; set; }
        public bool IsSubmitted { get; set; }
        public bool IsPosted { get; set; }
        public DateTime? DatePosted { get; set; }
        public string Feedback { get; set; }
        public PostType PostType { get; set; }
    }

    public enum PostType
    {
        BlogPost = 0,
        PositioningGuide = 1
    }
}
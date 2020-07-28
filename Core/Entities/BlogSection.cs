using System;

namespace Core.Entities
{
    public class BlogSection
    {
        public Guid Id { get; set; }
        public BlogSectionType Type { get; set; }
        public int Index { get; set; }
        public string Text { get; set; }
        public string ImageUrl { get; set; }
    }

    public enum BlogSectionType
    {
        Heading = 0,
        Paragraph = 1,
        Link = 2,
        ListItem = 3,
        Image = 4
    }
}
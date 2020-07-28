using Core.Entities;

namespace API.Dtos
{
    public class BlogSectionDto
    {
        public string Id { get; set; }
        public BlogSectionType Type { get; set; }
        public int Index { get; set; }
        public string Text { get; set; }
        public string ImageFile { get; set; }
        public string ImageUrl { get; set; }
    }
}
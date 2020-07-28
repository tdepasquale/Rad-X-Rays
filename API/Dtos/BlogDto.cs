using System;
using System.Collections.Generic;

namespace API.Dtos
{
    public class BlogDto
    {
        public string Id { get; set; }
        public string OwnerUsername { get; set; }
        public string Title { get; set; }
        public string CoverImageFile { get; set; }
        public string CoverImageUrl { get; set; }
        public List<BlogSectionDto> Sections { get; set; }
        public DateTime? DatePosted { get; set; }
        public string Feedback { get; set; }
    }
}
using System.Collections.Generic;

namespace API.Dtos
{
    public class PaginatedBlogDto
    {
        public List<BlogDto> Blogs { get; set; }
        public int NextPage { get; set; }
        public int TotalPages { get; set; }
    }
}
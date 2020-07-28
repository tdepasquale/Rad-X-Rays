using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Dtos;
using Core.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.BlogPosts
{
    public class ListSubmittedBlogPosts
    {
        public class Query : IRequest<List<BlogDto>>
        {
            public PostType PostType { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<BlogDto>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<List<BlogDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var submittedBlogsFromDB = await _context.Blogs.Where(x => x.IsSubmitted == true && x.IsPosted == false && x.PostType == request.PostType).Include(x => x.Sections).OrderByDescending(x => x.DatePosted).ToListAsync();

                var blogs = new List<BlogDto>();

                if (submittedBlogsFromDB == null) return blogs;

                foreach (var blog in submittedBlogsFromDB)
                {
                    var blogToAdd = new BlogDto
                    {
                        Id = blog.Id.ToString(),
                        OwnerUsername = blog.OwnerUsername,
                        Title = blog.Title,
                        CoverImageUrl = blog.CoverImageUrl
                    };

                    var sections = new List<BlogSectionDto>();

                    foreach (var section in blog.Sections)
                    {
                        var sectionToAdd = new BlogSectionDto
                        {
                            Id = section.Id.ToString(),
                            Index = section.Index,
                            ImageUrl = section.ImageUrl,
                            Text = section.Text,
                            Type = section.Type
                        };
                        sections.Add(sectionToAdd);
                    };

                    blogToAdd.Sections = sections.OrderBy(section => section.Index).ToList();

                    blogs.Add(blogToAdd);
                }

                return blogs;
            }
        }
    }
}
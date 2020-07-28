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
    public class ListPostedBlogPosts
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
                var submittedBlogsFromDB = await _context.Blogs.Where(x => x.IsSubmitted == true && x.IsPosted == true && x.PostType == request.PostType).OrderByDescending(x => x.DatePosted).ToListAsync();

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

                    blogs.Add(blogToAdd);
                }

                return blogs;
            }
        }
    }
}
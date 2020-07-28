using System;
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
    public class ListPaginatedBlogPosts
    {
        public class Query : IRequest<PaginatedBlogDto>
        {
            public PostType PostType { get; set; }
            public int Page { get; set; }
        }

        public class Handler : IRequestHandler<Query, PaginatedBlogDto>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<PaginatedBlogDto> Handle(Query request, CancellationToken cancellationToken)
            {
                const int pageSize = 1;
                var nextPage = 0;

                var totalBlogsFromDB = await _context.Blogs.Where(x => x.IsSubmitted == true && x.IsPosted == true && x.PostType == request.PostType).OrderByDescending(x => x.DatePosted).ToListAsync();

                var totalPages = totalBlogsFromDB.Count / pageSize;

                var submittedBlogsFromDB = totalBlogsFromDB.Skip(request.Page - 1 * pageSize).Take(pageSize);

                var blogs = new List<BlogDto>();

                var paginatedBlogInfo = new PaginatedBlogDto
                {
                    Blogs = new List<BlogDto>(),
                    NextPage = nextPage,
                    TotalPages = totalPages
                };

                if (submittedBlogsFromDB == null) return paginatedBlogInfo;

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

                if (blogs.Count < pageSize) nextPage = 0;
                else nextPage = request.Page + 1;

                paginatedBlogInfo.Blogs = blogs;
                paginatedBlogInfo.NextPage = nextPage;

                return paginatedBlogInfo;
            }
        }
    }
}
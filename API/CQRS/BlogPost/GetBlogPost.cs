using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Dtos;
using FluentValidation;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.BlogPosts
{
    public class GetBlogPost
    {
        public class Query : IRequest<BlogDto>
        {
            public Guid Id { get; set; }
        }

        public class RequestValidator : AbstractValidator<Query>
        {
            public RequestValidator()
            {
                RuleFor(x => x.Id).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Query, BlogDto>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<BlogDto> Handle(Query request, CancellationToken cancellationToken)
            {
                var blogFromDB = await _context.Blogs.Where(x => x.Id == request.Id && x.IsPosted == true && x.IsSubmitted == true).Include(x => x.Sections).SingleOrDefaultAsync();

                if (blogFromDB == null) return new BlogDto(); //throw error

                blogFromDB.Sections = blogFromDB.Sections.OrderBy(section => section.Index).ToList();

                var blog = new BlogDto
                {
                    Id = blogFromDB.Id.ToString(),
                    OwnerUsername = blogFromDB.OwnerUsername,
                    Title = blogFromDB.Title,
                    CoverImageUrl = blogFromDB.CoverImageUrl,
                    DatePosted = blogFromDB.DatePosted
                };

                var sections = new List<BlogSectionDto>();

                foreach (var section in blogFromDB.Sections)
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

                blog.Sections = sections;

                return blog;
            }
        }
    }
}
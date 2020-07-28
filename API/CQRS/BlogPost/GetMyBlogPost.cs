using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Dtos;
using Core.Entities;
using Core.Interfaces;
using FluentValidation;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.BlogPosts
{
    public class GetMyBlogPost
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
            private readonly UserManager<AppUser> _userManager;
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;
            public Handler(UserManager<AppUser> userManager, IUserAccessor userAccessor, DataContext context)
            {
                _context = context;
                _userAccessor = userAccessor;
                _userManager = userManager;
            }
            public async Task<BlogDto> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByNameAsync(_userAccessor.GetCurrentUsername());

                var blogFromDB = await _context.Blogs.Where(x => x.Id == request.Id && x.AppUserId == user.Id && x.IsSubmitted == false && x.IsPosted == false).Include(x => x.Sections).SingleOrDefaultAsync();

                if (blogFromDB == null)
                    return new BlogDto(); //throw error

                var blog = new BlogDto
                {
                    Id = blogFromDB.Id.ToString(),
                    OwnerUsername = blogFromDB.OwnerUsername,
                    Title = blogFromDB.Title,
                    CoverImageUrl = blogFromDB.CoverImageUrl,
                    DatePosted = blogFromDB.DatePosted,
                    Feedback = blogFromDB.Feedback
                };

                if (blogFromDB.Sections == null) return blog;

                blogFromDB.Sections = blogFromDB.Sections.OrderBy(section => section.Index).ToList();

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
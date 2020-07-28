using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using API.Dtos;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.BlogPosts
{
    public class CreateBlogPost
    {
        public class Command : IRequest<BlogDto>
        {
            public PostType PostType { get; set; }
        }


        public class Handler : IRequestHandler<Command, BlogDto>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            private readonly AppIdentityDbContext _identityContext;
            public Handler(AppIdentityDbContext identityContext, DataContext context, IUserAccessor userAccessor)
            {
                _identityContext = identityContext;
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<BlogDto> Handle(Command request, CancellationToken cancellationToken)
            {
                var fileManager = new S3FileManager();

                var user = await _identityContext.Users.SingleOrDefaultAsync(x =>
                    x.UserName == _userAccessor.GetCurrentUsername());

                var sections = new List<BlogSection> {
                    new BlogSection {Index = 1, Type = BlogSectionType.Paragraph, Text = "Blank Paragraph", ImageUrl=""}
                };

                var blog = new Blog
                {
                    AppUserId = user.Id,
                    OwnerUsername = user.UserName,
                    Sections = sections,
                    Title = "Blank Title",
                    CoverImageUrl = fileManager.DefaultImageUrl,
                    DatePosted = DateTime.Now,
                    IsSubmitted = false,
                    IsPosted = false,
                    Feedback = "",
                    PostType = request.PostType
                };

                _context.Blogs.Add(blog);

                var success = await _context.SaveChangesAsync() > 0;

                if (success)
                {
                    var blogDTO = new BlogDto { Id = blog.Id.ToString(), Title = blog.Title, OwnerUsername = blog.OwnerUsername, CoverImageUrl = blog.CoverImageUrl };
                    return blogDTO;
                }

                throw new Exception("Problem saving changes.");
            }
        }
    }
}
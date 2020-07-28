using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using Core.Entities;
using Core.Interfaces;
using FluentValidation;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.BlogPosts
{
    public class DeleteBlog
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Id).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly UserManager<AppUser> _userManager;
            private readonly IUserAccessor _userAccessor;
            public Handler(UserManager<AppUser> userManager, IUserAccessor userAccessor, DataContext context)
            {
                _userAccessor = userAccessor;
                _userManager = userManager;
                _context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByNameAsync(_userAccessor.GetCurrentUsername());

                var blog = await _context.Blogs.Where(x => x.Id == request.Id).Include(x => x.Sections).SingleOrDefaultAsync();

                if (blog == null)
                    throw new RestException(HttpStatusCode.NotFound, new { Blog = "Not found" });

                if (blog.AppUserId != user.Id || blog.IsSubmitted == true || blog.IsPosted == true)
                    throw new RestException(HttpStatusCode.BadRequest, new { Blog = "You do not have permission to delete this blog post." });

                var fileManager = new S3FileManager();

                await fileManager.DeleteImage(blog.CoverImageUrl, fileManager.BlogBucketName);

                foreach (var section in blog.Sections)
                {
                    if (section.Type == BlogSectionType.Image) await fileManager.DeleteImage(section.ImageUrl, fileManager.BlogBucketName);
                }

                _context.Remove(blog);

                var success = await _context.SaveChangesAsync() > 0;

                if (success) return Unit.Value;

                throw new Exception("Problem saving changes");
            }
        }
    }
}
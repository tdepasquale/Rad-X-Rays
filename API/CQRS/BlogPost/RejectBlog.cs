using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Errors;
using FluentValidation;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.BlogPosts
{
    public class RejectBlog
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
            public string Feedback { get; set; } = "";
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
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var blog = await _context.Blogs.Where(x => x.Id == request.Id).Include(x => x.Sections).SingleOrDefaultAsync();

                if (blog == null)
                    throw new RestException(HttpStatusCode.NotFound, new { Blog = "Not found" });

                blog.IsSubmitted = false;
                blog.IsPosted = false;
                blog.Feedback = request.Feedback;

                _context.Update(blog);

                var success = await _context.SaveChangesAsync() > 0;

                if (success) return Unit.Value;

                throw new Exception("Problem saving changes.");
            }
        }
    }
}
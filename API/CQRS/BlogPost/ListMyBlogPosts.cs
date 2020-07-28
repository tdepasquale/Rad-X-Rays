using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Dtos;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.BlogPosts
{
    public class ListMyBlogPosts
    {
        public class Query : IRequest<List<BlogDto>>
        {
            public PostType PostType { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<BlogDto>>
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
            public async Task<List<BlogDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByNameAsync(_userAccessor.GetCurrentUsername());

                var myBlogsFromDB = await _context.Blogs.Where(x => x.AppUserId == user.Id && x.IsSubmitted == false && x.IsPosted == false && x.PostType == request.PostType).OrderByDescending(x => x.DatePosted).ToListAsync();

                var blogs = new List<BlogDto>();

                if (myBlogsFromDB == null) return blogs;

                foreach (var blog in myBlogsFromDB)
                {
                    var blogToAdd = new BlogDto
                    {
                        Id = blog.Id.ToString(),
                        OwnerUsername = blog.OwnerUsername,
                        Title = blog.Title,
                        CoverImageUrl = blog.CoverImageUrl,
                        Feedback = blog.Feedback
                    };

                    blogs.Add(blogToAdd);
                }

                return blogs;
            }
        }
    }
}
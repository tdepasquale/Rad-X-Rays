using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using API.Dtos;
using Application.Errors;
using Core.Entities;
using Core.Interfaces;
using FluentValidation;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.BlogPosts
{
    public class UpdateBlogPost
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string CoverImageFile { get; set; }
            public List<BlogSectionDto> Sections { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Id).NotEmpty();
                RuleFor(x => x.Title).NotEmpty();
                RuleFor(x => x.Sections).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly DataContext _context;
            private readonly UserManager<AppUser> _userManager;
            private readonly IUserAccessor _userAccessor;
            private readonly ILogger<Handler> _logger;
            public Handler(UserManager<AppUser> userManager, IUserAccessor userAccessor, DataContext context, ILogger<Handler> logger)
            {
                _logger = logger;
                _userAccessor = userAccessor;
                _userManager = userManager;
                _context = context;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByNameAsync(_userAccessor.GetCurrentUsername());

                var blog = await _context.Blogs.Where(x => x.Id == request.Id).Include(x => x.Sections).SingleOrDefaultAsync();

                if (blog == null)
                    throw new RestException(HttpStatusCode.NotFound, new { BlogPost = "Not found." });

                if (blog.AppUserId != user.Id)
                    throw new RestException(HttpStatusCode.BadRequest, new { BlogPost = "You do not have permission to edit this blog post." });

                blog.Title = request.Title;

                var fileManager = new S3FileManager();

                blog.CoverImageUrl = await fileManager.TryImageUpload(request.CoverImageFile, blog.CoverImageUrl);

                var updatedSections = new List<BlogSection>();
                var deletedImageSections = blog.Sections.Where(x => x.Type == BlogSectionType.Image);

                foreach (var sectionDTO in request.Sections)
                {
                    Guid id = Guid.NewGuid();
                    Guid.TryParse(sectionDTO.Id, out id);

                    int maxTextLength = 0;

                    switch (sectionDTO.Type)
                    {
                        case BlogSectionType.Heading:
                            if (sectionDTO.Text.Length > 50) maxTextLength = 50;
                            else maxTextLength = sectionDTO.Text.Length;
                            break;
                        case BlogSectionType.Image:
                            if (sectionDTO.Text.Length > 100) maxTextLength = 100;
                            else maxTextLength = sectionDTO.Text.Length;
                            break;
                        case BlogSectionType.Paragraph:
                            if (sectionDTO.Text.Length > 5000) maxTextLength = 5000;
                            else maxTextLength = sectionDTO.Text.Length;
                            break;
                        case BlogSectionType.Link:
                            if (sectionDTO.Text.Length > 50) maxTextLength = 50;
                            else maxTextLength = sectionDTO.Text.Length;
                            break;
                        case BlogSectionType.ListItem:
                            if (sectionDTO.Text.Length > 5000) maxTextLength = 5000;
                            else maxTextLength = sectionDTO.Text.Length;
                            break;
                        default:
                            maxTextLength = 0;
                            break;
                    }

                    var newSection = new BlogSection
                    {
                        Id = id,
                        Index = sectionDTO.Index,
                        Type = sectionDTO.Type,
                        Text = sectionDTO.Text.Substring(0, maxTextLength)
                    };

                    if (sectionDTO.Type == BlogSectionType.Image)
                    {
                        deletedImageSections = deletedImageSections.Where(x => x.Id.ToString() != sectionDTO.Id);
                        var oldImageURL = sectionDTO.ImageUrl;
                        var oldImageSection = blog.Sections.Where(x => x.Id.ToString() == sectionDTO.Id).SingleOrDefault();
                        if (oldImageSection != null) oldImageURL = oldImageSection.ImageUrl;
                        newSection.ImageUrl = await fileManager.TryImageUpload(sectionDTO.ImageFile, oldImageURL);
                    }

                    updatedSections.Add(newSection);
                }

                blog.Sections = updatedSections;

                foreach (var deletedImageSection in deletedImageSections)
                {
                    await fileManager.DeleteImage(deletedImageSection.ImageUrl, fileManager.BlogBucketName);
                }

                _context.Blogs.Update(blog);

                var success = await _context.SaveChangesAsync() > 0;

                if (success) return Unit.Value;

                throw new Exception("Problem saving changes");
            }
        }

    }
}
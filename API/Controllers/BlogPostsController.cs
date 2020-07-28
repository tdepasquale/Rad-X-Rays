using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Dtos;
using Application.BlogPosts;
using Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    public class BlogPostsController : BaseApiController
    {
        private readonly ILogger<BlogPostsController> _logger;
        public BlogPostsController(ILogger<BlogPostsController> logger)
        {
            _logger = logger;
        }

        [Authorize(Roles = "Writer")]
        [HttpGet("list-my-blogs")]
        public async Task<ActionResult<List<BlogDto>>> ListMyBlogs()
        {
            return await Mediator.Send(new ListMyBlogPosts.Query { PostType = PostType.BlogPost });
        }

        [Authorize(Roles = "Writer")]
        [HttpGet("list-my-positioning-guides")]
        public async Task<ActionResult<List<BlogDto>>> ListMyPositioningGuides()
        {
            return await Mediator.Send(new ListMyBlogPosts.Query { PostType = PostType.PositioningGuide });
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("list-submitted-blogs")]
        public async Task<ActionResult<List<BlogDto>>> ListSubmittedBlogs()
        {
            return await Mediator.Send(new ListSubmittedBlogPosts.Query { PostType = PostType.BlogPost });
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("list-submitted-positioning-guides")]
        public async Task<ActionResult<List<BlogDto>>> ListSubmittedPositioningGuides()
        {
            return await Mediator.Send(new ListSubmittedBlogPosts.Query { PostType = PostType.PositioningGuide });
        }

        [AllowAnonymous]
        [HttpGet("list-posted-blogs")]
        public async Task<ActionResult<List<BlogDto>>> ListPostedBlogs()
        {
            return await Mediator.Send(new ListPostedBlogPosts.Query { PostType = PostType.BlogPost });
        }

        [AllowAnonymous]
        [HttpGet("list-paginated-blogs/{page}")]
        public async Task<ActionResult<PaginatedBlogDto>> ListPaginatedBlogs(int page)
        {
            _logger.LogInformation($"PAGE NUMBER: {page}");
            return await Mediator.Send(new ListPaginatedBlogPosts.Query { PostType = PostType.BlogPost, Page = page });
        }

        [AllowAnonymous]
        [HttpGet("list-posted-positioning-guides")]
        public async Task<ActionResult<List<BlogDto>>> ListPostedPositioningGuides()
        {
            return await Mediator.Send(new ListPostedBlogPosts.Query { PostType = PostType.PositioningGuide });
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<BlogDto>> GetPostedBlog(Guid id)
        {
            return await Mediator.Send(new GetBlogPost.Query { Id = id });
        }

        [Authorize(Roles = "Writer")]
        [HttpGet("get-my-blog/{id}")]
        public async Task<ActionResult<BlogDto>> GetMyBlog(Guid id)
        {
            return await Mediator.Send(new GetMyBlogPost.Query { Id = id });
        }

        [Authorize(Roles = "Writer")]
        [HttpPost("create-blog")]
        public async Task<ActionResult<BlogDto>> CreateBlog()
        {
            return await Mediator.Send(new CreateBlogPost.Command { PostType = PostType.BlogPost });
            // return await Mediator.Send(command);
        }

        [Authorize(Roles = "Writer")]
        [HttpPost("create-positioning-guide")]
        public async Task<ActionResult<BlogDto>> CreatePositioningGuide()
        {
            return await Mediator.Send(new CreateBlogPost.Command { PostType = PostType.PositioningGuide });
        }

        [Authorize(Roles = "Writer")]
        [HttpPut("edit")]
        public async Task<ActionResult<Unit>> Edit(UpdateBlogPost.Command command)
        {
            return await Mediator.Send(command);
        }

        [Authorize(Roles = "Writer")]
        [HttpPost("submit/{id}")]
        public async Task<ActionResult<Unit>> Submit(Guid id)
        {
            return await Mediator.Send(new SubmitBlog.Command { Id = id });
        }

        [Authorize(Roles = "Writer")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> DeleteBlog(Guid id)
        {
            return await Mediator.Send(new DeleteBlog.Command { Id = id });
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("approve-blog/{id}")]
        public async Task<ActionResult<Unit>> ApproveBlog(Guid id)
        {
            return await Mediator.Send(new ApproveBlog.Command { Id = id });
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("reject-blog")]
        public async Task<ActionResult<Unit>> RejectBlog(RejectBlog.Command command)
        {
            return await Mediator.Send(command);
        }

    }
}
using System.Net;
using System.Threading.Tasks;
using API.CQRS;
using API.Dtos;
using Application.Errors;
using Core.Entities;
using Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [AllowAnonymous]
    public class UserController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IUserAccessor _userAccessor;
        private readonly IFacebookAccessor _facebookAccessor;
        private readonly ILogger<UserController> _logger;

        public UserController(UserManager<AppUser> userManager, IJwtGenerator jwtGenerator, IUserAccessor userAccessor, IFacebookAccessor facebookAccessor, ILogger<UserController> logger)
        {
            _logger = logger;
            _facebookAccessor = facebookAccessor;
            _userAccessor = userAccessor;
            _jwtGenerator = jwtGenerator;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> CurrentUser()
        {
            var name = _userAccessor.GetCurrentUsername();
            if (name == null) throw new RestException(HttpStatusCode.NotFound, new { User = "Not found" });
            var user = await _userManager.FindByNameAsync(name);
            var roles = await _userManager.GetRolesAsync(user);

            return new UserDto
            {
                DisplayName = user.DisplayName,
                Username = user.UserName,
                Token = _jwtGenerator.CreateToken(user, roles),
                Roles = roles
            };
        }

        [AllowAnonymous]
        [HttpPost("facebook")]
        public async Task<ActionResult<UserDto>> FacebookLogin(FacebookLogin.Query query)
        {
            return await Mediator.Send(query);
        }

        [AllowAnonymous]
        [HttpPost("google")]
        public async Task<ActionResult<UserDto>> GoogleLogin(GoogleLogin.Query query)
        {
            return await Mediator.Send(query);
        }

        // [AllowAnonymous]
        // [HttpPost("facebook")]
        // public async Task<ActionResult<UserDto>> FacebookLogin(string accessToken)
        // {
        //     var userInfo = await _facebookAccessor.FacebookLogin(accessToken);

        //     _logger.LogInformation($"Access Token: {accessToken}, userInfo: {userInfo}");

        //     if (userInfo == null) return BadRequest($"Problem validating token. {accessToken}");

        //     var user = await _userManager.FindByEmailAsync(userInfo.Email);
        //     var roles = await _userManager.GetRolesAsync(user);

        //     if (user == null)
        //     {
        //         user = new AppUser
        //         {
        //             DisplayName = userInfo.Name,
        //             Id = userInfo.Id,
        //             Email = userInfo.Email,
        //             UserName = "fb_" + userInfo.Id
        //         };

        //         var result = await _userManager.CreateAsync(user);
        //         if (!result.Succeeded) return BadRequest("Problem creating user.");
        //     }

        //     return new UserDto
        //     {
        //         DisplayName = user.DisplayName,
        //         Token = _jwtGenerator.CreateToken(user, roles),
        //         Username = user.UserName,
        //         Roles = roles
        //     };
        // }
    }
}
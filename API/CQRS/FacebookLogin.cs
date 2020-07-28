using System.Net;
using System.Threading;
using System.Threading.Tasks;
using API.Dtos;
using Application.Errors;
using Core.Entities;
using Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace API.CQRS
{
    public class FacebookLogin
    {
        public class Query : IRequest<UserDto>
        {
            public string AccessToken { get; set; }
        }

        public class Handler : IRequestHandler<Query, UserDto>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IFacebookAccessor _facebookAccessor;
            private readonly IJwtGenerator _jwtGenerator;
            private readonly ILogger<Handler> _logger;
            public Handler(UserManager<AppUser> userManager, IFacebookAccessor facebookAccessor, IJwtGenerator jwtGenerator, ILogger<Handler> logger)
            {
                _logger = logger;
                _jwtGenerator = jwtGenerator;
                _facebookAccessor = facebookAccessor;
                _userManager = userManager;
            }

            public async Task<UserDto> Handle(Query request, CancellationToken cancellationToken)
            {
                _logger.LogInformation($"THE ACCESS TOKEN IS: {request.AccessToken}");

                var userInfo = await _facebookAccessor.FacebookLogin(request.AccessToken);

                if (userInfo == null) throw new RestException(HttpStatusCode.BadRequest, "Problem validating token.");

                var user = await _userManager.FindByEmailAsync(userInfo.Email);
                var roles = await _userManager.GetRolesAsync(user);

                if (user == null)
                {
                    user = new AppUser
                    {
                        DisplayName = userInfo.Name,
                        Id = userInfo.Id,
                        Email = userInfo.Email,
                        UserName = "fb_" + userInfo.Id
                    };

                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded) throw new RestException(HttpStatusCode.BadRequest, new { User = "Problem creating user." });
                }

                return new UserDto
                {
                    DisplayName = user.DisplayName,
                    Token = _jwtGenerator.CreateToken(user, roles),
                    Username = user.UserName,
                    Roles = roles
                };

            }
        }
    }
}
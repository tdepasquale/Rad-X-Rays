using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Google.Apis.Auth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Infrastructure.Security
{
    public class GoogleAccessor : IGoogleAccessor
    {
        private readonly IOptions<GoogleAppSettings> _config;
        private readonly ILogger<GoogleAccessor> _logger;

        public GoogleAccessor(IOptions<GoogleAppSettings> config, ILogger<GoogleAccessor> logger)
        {
            _config = config;
            _logger = logger;
        }

        public GoogleUserInfo GoogleLogin(string tokenId)
        {
            try
            {
                var payload = GoogleJsonWebSignature.ValidateAsync(tokenId,
                    new GoogleJsonWebSignature.ValidationSettings()
                    {
                        Audience = new List<string> { _config.Value.ClientId },
                    }).Result;

                if (payload.Email == null) throw new NullReferenceException();

                var result = new GoogleUserInfo()
                {
                    Id = payload.JwtId,
                    Name = payload.Name,
                    Email = payload.Email
                };

                return result;
            }
            catch (Exception error)
            {
                _logger.LogError(0, error, "Error logging in with google.");
                return null;
            }
        }

    }
}
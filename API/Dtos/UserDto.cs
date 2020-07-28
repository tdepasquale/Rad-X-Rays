using System.Collections.Generic;

namespace API.Dtos
{
    public class UserDto
    {
        public string DisplayName { get; set; }
        public string Token { get; set; }
        public string Username { get; set; }
        public IList<string> Roles { get; set; }
    }
}
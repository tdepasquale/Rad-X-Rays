using System.Collections.Generic;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IJwtGenerator
    {
        string CreateToken(AppUser user, IList<string> roles);
    }
}
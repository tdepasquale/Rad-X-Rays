using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IFacebookAccessor
    {
        Task<FacebookUserInfo> FacebookLogin(string accessToken);
    }
}
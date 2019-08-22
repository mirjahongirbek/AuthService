using AuthService.Interfaces.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthService.Interfaces.Service
{
    public interface IAuthRepository<TUser, TRole, TKey>
        where TUser : class, IUser<TRole, TKey>
        where TRole:class, IRole<TKey>
    {
        Task<TUser> GetLoginOrEmail(string model);
        Task<TUser> GetUser(TUser model);
        Task<bool> IsLoginedAsync(TUser model);
        Task<bool> RegisterAsync(TUser model);
        Task Logout(string access);
        Task Delete(TKey id);
        Task<TUser> GetMe(string id);
       Task<ClaimsIdentity> Login(string username, string password);

    }
}

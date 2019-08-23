
using AuthService.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthService.Interfaces.Service
{
    public interface IAuthRepository<TUser, TRole, TKey>
        where TUser : EntityUser<TRole>
        where TRole:EntityUserRole
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

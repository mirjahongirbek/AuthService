using AuthService.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthService.Interfaces.Service
{
    public interface IAuthRepository<TUser, TUserRole>
       where TUser : IdentityUser
        where TUserRole : IdentityUserRole
    {
        Task<TUser> GetLoginOrEmail(string model);
        Task<bool> RegisterAsync(TUser model);
        Task Logout(string access);
        Task<TUser> GetByUserName(string userName);
        DbSet<TUser> DbSet { get; }
        Task<bool> Delete(int id);
        Task<TUser> GetMe(string id);
       Task<ClaimsIdentity> LoginClaims(string username, string password);
        Task<bool> Delete(TUser user);
        Task<LoginResult> Login(string username, string password);


    }
}

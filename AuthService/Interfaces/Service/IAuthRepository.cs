using AuthService.Models;
using AuthService.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthService.Interfaces.Service
{
    public interface IAuthRepository<TUser, TUserRole>
       where TUser : IdentityUser
        where TUserRole : IdentityUserRole
    {
        #region Get
        Task<TUser> GetByEmail(string model);
        Task<TUser> GetByUserName(string userName);
        Task<TUser> GetMe(int id);
        TUser Get(int id);
        Task<TUser> GetMe(string id);
        TUser GetFirst(Expression<Func<TUser, bool>> expression);
        #endregion
        #region  Add
        bool AddUser(TUser user);
        Task<bool> RegisterAsync(TUser model);
        #endregion



        Task Logout(string access);
               DbSet<TUser> DbSet { get; }
        Task<bool> Delete(int id);

        LoginResult Login(TUser user);
        Task Update(TUser user);
        IEnumerable<TUser> FindAll();
        IEnumerable<TUser> Find(Expression<Func<TUser, bool>> expression);
        long Count();
        long Count(Expression<Func<TUser, bool>> expression);
        Task<ClaimsIdentity> LoginClaims(string username, string password);
        Task<bool> Delete(TUser user);
        Task<(LoginResult, TUser)> Login(LoginViewModal model);
        Task<(LoginResult, TUser)> Login(string username, string password);
        void SetRefresh(TUser user);
        string SetToken(List<Claim> claims, TUser user);
        #region Check
        TUser CheckUser(string userName);
        TUser CheckUserByPhone(string userName, string phoneNumber);
        TUser CheckUserByP(string userName, string Password);
        #endregion
        #region Otp
        bool CheckOtp(TUser user, string otp);
        bool CheckOtp(ClaimsPrincipal user, string otp);
        void SetOtp(ClaimsPrincipal user, string Otp);
        void SetOtp(int UserId, string otp);
        void SetOtp(string username, string otp);
        bool SetOtp(TUser user, string otp);
        #endregion

    }
}

using AuthService.Interfaces.Models;
using AuthService.Interfaces.Service;
using EntityRepository.Context;
using Microsoft.EntityFrameworkCore;
using RepositoryCore.Attributes;
using RepositoryCore.CoreState;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthService.Service
{
    public class AuthRepository<TUser, TUserRole, TRole> : IAuthRepository<TUser, TRole, int>
        where TUser : User<TUserRole, int>
        where TRole : Role<int>
        where TUserRole : UserRole<int>
    {
        private DbSet<TUser> _dbSet;
        private DbContext _context;

        public AuthRepository(IDbContext dbContext)
        {
            _dbSet = dbContext.DataContext.Set<TUser>();
            _context = dbContext.DataContext;
        }

        public Task Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<TUser> GetLoginOrEmail(string model)
        {
            throw new System.NotImplementedException();
        }

        public Task<TUser> GetMe(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<TUser> GetUser(TUser model)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> IsLoginedAsync(TUser model)
        {
            throw new System.NotImplementedException();
        }


        public async Task<ClaimsIdentity> Login(string username, string password)
        {
            var user = await _dbSet.FirstOrDefaultAsync(m => m.UserName == username
            && m.Password == CoreState.GetHashString(password));
            if (user.UserRoles == null)
            {
                var usrRoles = _context.Set<TUserRole>();
                var userRoles = usrRoles.Where(mbox => mbox.UserId == user.Id).ToList();
                user.UserRoles = userRoles;
            }
        }
        private List<Claim> Claims(TUser user)
        {
            var usr = user.GetType();
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("Id", user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim("Position", user.Position.ToString()));
            claims.Add(new Claim("Email", user.Email));
            claims.Add(new user.Position.ToString())
            foreach (var i in user.UserRoles)
            {
                new Claim(ClaimTypes.Role, i.Role.Name);
            }
            foreach (var i in usr.GetProperties())
            {
                var token = i.GetCustomAttribute<TokenAttribute>();
                if (token == null)
                    continue;

               var name= string.IsNullOrEmpty(token.Name) ? i.Name : token.Name;
               if( claims.FirstOrDefault(m => m.Type == name) == null)
                {
                    claims.Add(new Claim(name, i.GetValue(user).ToString()));
                }


            }
            return claims;
        }

        public Task Logout(string access)
        {
            throw new System.NotImplementedException();
        }
        public async Task<bool> RegisterAsync(TUser model)
        {
            var user = await _dbSet.FirstOrDefaultAsync(m => m.UserName == model.UserName && m.Password == CoreState.GetHashString(model.Password));
            if (user != null) { return false; }

            _dbSet.Add(model);
            return true;
        }

    }
}

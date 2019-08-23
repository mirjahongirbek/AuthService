using AuthService.Models;
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
    public class AuthRepository<TUser, TUserRole, TRole> 
      where TRole :EntityRole
        where TUserRole :EntityUserRole
          where TUser :  EntityUser<TUserRole>
    {
        private DbSet<TUser> _dbSet;
        private DbContext _context;

        public AuthRepository(IDbContext dbContext)
        {
            _dbSet = dbContext.DataContext.Set<TUser>();
            _context = dbContext.DataContext;
        }

        public virtual Task Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task<TUser> GetLoginOrEmail(string model)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task<TUser> GetMe(string id)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task<TUser> GetUser(TUser model)
        {
            throw new System.NotImplementedException();
        }

        public virtual Task<bool> IsLoginedAsync(TUser model)
        {
            throw new System.NotImplementedException();
        }


        public virtual async Task<ClaimsIdentity> Login(string username, string password)
        {
            var user = await _dbSet.FirstOrDefaultAsync(m => m.UserName == username
            && m.Password == CoreState.GetHashString(password));
            if(user== null) { return null; }
            if (user.UserRoles == null)
            {
                var usrRoles = _context.Set<TUserRole>();
                var userRoles = usrRoles.Where(mbox => mbox.UserId == user.Id).ToList();
                user.UserRoles = userRoles;
            }
            var clams= Claims(user);
            if(clams== null)
            {
                return null;
            }
            var claimsIdentity = new ClaimsIdentity(clams, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
        private List<Claim> Claims(TUser user)
        {
            var usr = user.GetType();
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("Id", user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim("Position", user.Position.ToString()));
            claims.Add(new Claim("Email", user.Email));
            claims.Add(new Claim("Position",user.Position.ToString()));
            foreach (var role in user.UserRoles)
            {
                claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Role.Name));

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

        public virtual async Task Logout(string access)
        {
            
        }
        public virtual async Task<bool> RegisterAsync(TUser model)
        {


            var user = await _dbSet.FirstOrDefaultAsync(m => m.UserName == model.UserName && m.Password == CoreState.GetHashString(model.Password));
            if (user != null) { return false; }
            _dbSet.Add(model);
            return true;
        }

    }
}

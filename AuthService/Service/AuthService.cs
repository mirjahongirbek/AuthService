using AuthService.Interfaces.Service;
using AuthService.Models;
using EntityRepository.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RepositoryCore.Attributes;
using RepositoryCore.CoreState;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthService.Service
{
    public class AuthRepository<TUser, TUserRole, TRole>
        : IAuthRepository<TUser, TUserRole, int>
      where TRole : EntityRole
      where TUserRole : EntityUserRole
      where TUser : EntityUser<TUserRole>
    {
        private DbSet<TUser> _dbSet;
        private DbContext _context;
        public DbSet<TUser> DbSet => _dbSet;
        public AuthRepository(IDbContext dbContext)
        {
            _dbSet = dbContext.DataContext.Set<TUser>();
            _context = dbContext.DataContext;
        }
        public virtual async Task<bool> Delete(int id)
        {
            var user = Get(id);
            if (user == null)
            {
                return false;
            }
            _dbSet.Remove(user);
            return true;
        }
        public virtual TUser Get(int id)
        {
            return _dbSet.FirstOrDefault(m => m.Id == id);
        }
        public virtual async Task<TUser> GetLoginOrEmail(string email)
        {
            return _dbSet.FirstOrDefault(m => m.Email == email || m.UserName == email);
        }
        public virtual async Task<TUser> GetByUserName(string userName)
        {
            return _dbSet.FirstOrDefault(m => m.UserName == userName);
        }
        public virtual async Task<ClaimsIdentity> LoginClaims(string username, string password)
        {
            var user = await _dbSet.FirstOrDefaultAsync(m => m.UserName == username
            && m.Password == RepositoryState.GetHashString(password));
            if (user == null) { return null; }
            if (user.UserRoles == null)
            {
                var usrRoles = _context.Set<TUserRole>();
                var userRoles = usrRoles.Where(mbox => mbox.UserId == user.Id).ToList();
                user.UserRoles = userRoles;
            }
            var clams = Claims(user);
            if (clams == null)
            {
                return null;
            }
            var claimsIdentity = new ClaimsIdentity(clams, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
        protected virtual List<Claim> Claims(TUser user)
        {
            var usr = user.GetType();
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("Id", user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim("Position", user.Position.ToString()));
            claims.Add(new Claim("Email", user.Email));
            claims.Add(new Claim("Position", user.Position.ToString()));
            foreach (var role in user.UserRoles)
            {
                claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Role.Name));

            }
            foreach (var i in usr.GetProperties())
            {
                var token = i.GetCustomAttribute<TokenAttribute>();
                if (token == null)
                    continue;

                var name = string.IsNullOrEmpty(token.Name) ? i.Name : token.Name;
                if (claims.FirstOrDefault(m => m.Type == name) == null)
                {
                    claims.Add(new Claim(name, i.GetValue(user).ToString()));
                }


            }
            return claims;
        }
        public virtual async Task Logout(string access)
        {
            throw new NotImplementedException();
        }
        public virtual async Task<bool> RegisterAsync(TUser model)
        {
            var user = await
                _dbSet.FirstOrDefaultAsync(m => m.UserName == model.UserName && m.Password == RepositoryState.GetHashString(model.Password));
            if (user != null) { return false; }
            _dbSet.Add(model);
            return true;
        }
        public async Task<bool> Delete(TUser user)
        {
            _dbSet.Remove(user);
            return true;
        }
        public async Task<TUser> GetMe(string userName)
        {
            return _dbSet.FirstOrDefault(m => m.UserName == userName);
        }
        public async Task Update(TUser user)
        {
            _dbSet.Update(user);

        }
        public virtual async Task<LoginResult> Login(string username, string password)
        {
            LoginResult loginResult = new LoginResult();
            var now = DateTime.UtcNow;
            var user = await _dbSet.FirstOrDefaultAsync(m => m.UserName == username
             && m.Password == RepositoryState.GetHashString(password));
            if (user == null) { return null; }
            if (user.UserRoles == null)
            {
                var usrRoles = _context.Set<TUserRole>();
                var userRoles = usrRoles.Where(mbox => mbox.UserId == user.Id).ToList();
                user.UserRoles = userRoles;
            }
            var clams = Claims(user);
            var claimsIdentity = new ClaimsIdentity(clams, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            var jwt = new JwtSecurityToken(
                AuthOptions.ISSUER,
                AuthOptions.AUDIENCE,
                notBefore: now,
                claims: claimsIdentity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            user.Token = encodedJwt;
            var random = new Random();
            var refresh = "";
            for (var i = 0; i < 10; i++) refresh += i.ToString();
            user.RefreshToken = RepositoryState.GetHashString(encodedJwt + refresh);
            user.LastDate = DateTime.Now;
            await Update(user);
            loginResult.AccessToken = encodedJwt;
            loginResult.UserName = claimsIdentity.Name;
            loginResult.RefreshToken = user.RefreshToken;
            loginResult.Roles = user.UserRoles.Select(mbox => mbox.Role.Name).ToList();
            return loginResult;
        }
    }
}

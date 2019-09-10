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
    public  class IdentityUserService<TUser,TRole, TUserRole>: IAuthRepository<TUser, TUserRole>
        where TUser : IdentityUser
        where TUserRole:IdentityUserRole
        where TRole: IdentityRole
    {
        DbSet<TUser> _dbSet;
        DbSet<TUserRole> _userRole;
        DbContext _context;
        public DbSet<TUser> DbSet { get; }
        IRoleRepository<TRole> _roleService;
        public IdentityUserService(IDbContext context, IRoleRepository<TRole> roleService)
        {
            _dbSet = context.DataContext.Set<TUser>();
            _userRole = context.DataContext.Set<TUserRole>();
            _context = context.DataContext;
            _roleService = roleService;

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
            && m.PasswordHash == RepositoryState.GetHashString(password));
            if (user == null) { return null; }
            _userRole.Where(m => m.UserId == user.Id);
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
            claims.Add(new Claim("Email", user.Email??""));
            var roles= GetRoles(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Name));
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
                _dbSet.FirstOrDefaultAsync(m => m.UserName == model.UserName 
                && m.PasswordHash == RepositoryState.GetHashString(model.PasswordHash));
            if (user != null) { return false; }
            model.PasswordHash = RepositoryState.GetHashString(model.PasswordHash);
            _dbSet.Add(model);
            _context.SaveChanges();
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
            _context.SaveChanges();
        }
        List<TRole> GetRoles(TUser user)
        {
            List<TUserRole> userRoles = _userRole.Where(m => m.UserId == user.Id).ToList();
            var IdRoles = userRoles.Select(m => m.RoleId).ToList();
            var roles = _roleService.GetList(IdRoles);
            return roles;
        }
        public virtual async Task<LoginResult> Login(string username, string password)
        {
            LoginResult loginResult = new LoginResult();
            var now = DateTime.UtcNow;
           var r= _dbSet.Where(m => true).ToList();
            var user =_dbSet.Where(m => m.UserName == username
             && m.PasswordHash == RepositoryState.GetHashString(password)).FirstOrDefault();
            if (user == null) { return null; }
           var roles= GetRoles(user);

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
            user.LastLoginDate = DateTime.Now;
            await Update(user);
            loginResult.AccessToken = encodedJwt;
            loginResult.UserName = claimsIdentity.Name;
            loginResult.RefreshToken = user.RefreshToken;
            loginResult.Roles = roles.Select(m => m.Name).ToList();
            //TODO
            return loginResult;
        }
    }
}


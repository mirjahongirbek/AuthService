using AuthService.Attributes;
using AuthService.Interfaces.Service;
using AuthService.Models;
using AuthService.Models.Models;
using EntityRepository.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using RepositoryCore.CoreState;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthService.Service
{
    public class IdentityUserService<TUser, TRole, TUserRole> : IAuthRepository<TUser, TUserRole>
        where TUser : IdentityUser
        where TUserRole : IdentityUserRole
        where TRole : IdentityRole
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
        public virtual async Task<TUser> GetByEmail(string email)
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
            _userRole.Where(m => m.UserId == user.Id);
            var clams = Claims(user);
            if (clams == null)
            {
                return null;
            }
            var claimsIdentity = new ClaimsIdentity(clams, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
        public virtual List<Claim> Claims(TUser user)
        {
            var usr = user.GetType();
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("Id", user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim("Position", user.Position.ToString()));
            claims.Add(new Claim("Email", user.Email ?? ""));
            var roles = GetRoles(user);
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
                    if(i.GetValue(user)!= null)
                    claims.Add(new Claim(name, i.GetValue(user).ToString()));
                }


            }
            return claims;
        }
        //TODO
        public virtual async Task Logout(string access)
        {
            throw new NotImplementedException();
        }
        public virtual async Task<bool> RegisterAsync(TUser model)
        {
            var user = await
                _dbSet.FirstOrDefaultAsync(m => m.UserName == model.UserName
                && m.Password == RepositoryState.GetHashString(model.Password));
            if (user != null) { return false; }
            model.Password = RepositoryState.GetHashString(model.Password);
            _dbSet.Add(model);
            _context.SaveChanges();
            return true;
        }
        public async Task<bool> Delete(TUser user)
        {
            _dbSet.Remove(user);
            Save();
            return true;
        }
        protected void Save()
        {
            _context.SaveChanges();
        }
        public async Task<TUser> GetMe(string userName)
        {
            return _dbSet.FirstOrDefault(m => m.UserName == userName);
        }
        public async Task Update(TUser user)
        {
            _dbSet.Update(user);
            Save();
        }
        public List<TRole> GetRoles(TUser user)
        {
            List<TUserRole> userRoles = _userRole.Where(m => m.UserId == user.Id).ToList();
            var IdRoles = userRoles.Select(m => m.RoleId).ToList();
            var roles = _roleService.GetList(IdRoles);
            return roles;
        }
        public virtual async Task<(LoginResult, TUser)> Login(LoginViewModal model)
        {
            var user = _dbSet.Where(m => m.UserName == model.UserName && m.Password == RepositoryState.GetHashString(model.Password)).FirstOrDefault();
            if (user == null)
            {
                return (null, null);
            }
            if (AuthOptions.CheckDeviceId)
            {
                if(user.DeviceList.Contains(model.DeviceId))
                return (Login(user), user);
                return (null, user);
            }
            return (Login(user), user);/*
            if (user.DeviceList.Contains(model.DeviceId) && AuthOptions.CheckDeviceId)
            {
                return (Login(user), user);
            }
            return (null, user);*/
        }
        public virtual async Task<(LoginResult, TUser)> Login(string username, string password)
        {
            var user = _dbSet.Where(m => m.UserName == username && m.Password == RepositoryState.GetHashString(password)).FirstOrDefault();
            if (user == null)
            {

            }
            return (Login(user), user);
        }
        public virtual LoginResult Login(TUser user)
        {
            if (user == null) { return null; }
            var roles = GetRoles(user);
            SetToken(Claims(user), user);
            Update(user).Wait();
            LoginResult loginResult = new LoginResult()
            {
                AccessToken = user.Token,
                UserName = user.UserName,
                RefreshToken = user.RefreshToken,
                MyId= user.Id,
                Roles = roles.Select(m => m.Name).ToList()
            };
            return loginResult;
        }
        public void SetRefresh(TUser user)
        {
            var refresh = "";
            var random = new Random();
            for (var i = 0; i < 10; i++) refresh += random.Next(15);
            user.RefreshToken = RepositoryState.GetHashString(refresh);
        }
        public string SetToken(List<Claim> claims, TUser user = null)
        {
            var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            var now = DateTime.Now;
            var jwt = new JwtSecurityToken(
                 AuthOptions.ISSUER,
                 AuthOptions.AUDIENCE,
                 notBefore: now,
                 claims: claimsIdentity.Claims,
                 expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                 signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                     SecurityAlgorithms.HmacSha256));
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            if (user != null)
            {
                user.Token = token;
                user.LastLoginDate = DateTime.Now;
            }

            return token;
        }
        public async Task<TUser> GetMe(int id)
        {
            return _dbSet.FirstOrDefault(m => m.Id == id);
        }
        public bool AddUser(TUser user)
        {
            _dbSet.Add(user);
            Save();
            return true;
        }
        public IEnumerable<TUser> FindAll()
        {
            return _dbSet.Where(m => true);
        }
        public TUser CheckUser(string userName)
        {
            return _dbSet.FirstOrDefault(m => m.UserName == userName);

        }
        public TUser CheckUserByPhone(string userName, string phoneNumber)
        {
            var user = _dbSet.FirstOrDefault(m => m.UserName == userName && m.PhoneNumber == phoneNumber);
            return user;
        }
        public TUser CheckUserByP(string userName, string Password)
        {
            return _dbSet.FirstOrDefault(m => m.UserName == userName && m.Password == RepositoryState.GetHashString(Password));
        }
        public TUser GetFirst(Expression<Func<TUser, bool>> expression)
        {
            return _dbSet.FirstOrDefault(expression);
        }
        public IEnumerable<TUser> Find(Expression<Func<TUser, bool>> expression)
        {
            return _dbSet.Where(expression);
        }
        public long Count()
        {
            return _dbSet.Count();
        }
        #region Otp
        public bool CheckOtp(TUser user, string otp)
        {
            if (user.LastOtp == otp)
            {
                return true;
            }
            return false;
        }

        public bool CheckOtp(ClaimsPrincipal claims, string otp)
        {
            var user = GetByUserName(claims.Identity.Name).Result;
            return CheckOtp(user, otp);
        }

        public void SetOtp(ClaimsPrincipal claims, string otp)
        {
            var user = GetByUserName(claims.Identity.Name).Result;
            SetOtp(user, otp);
        }

        public bool SetOtp(TUser user, string otp)
        {
            user.LastOtpDate = DateTime.Now;
            user.LastOtp = otp;
            Update(user).Wait();
            return true;
        }
        public void SetOtp(int id, string otp)
        {
            SetOtp(GetMe(id).Result, otp);
        }
        public void SetOtp(string username, string otp)
        {
            SetOtp(GetByUserName(username).Result, otp);

        }

        public long Count(Expression<Func<TUser, bool>> expression)
        {
           return _dbSet.Count(expression);
        }
        #endregion



    }
}


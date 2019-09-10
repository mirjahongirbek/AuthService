using AuthService.Interfaces.Service;
using AuthService.Models;
using Microsoft.AspNetCore.Mvc;
using TestApi.Entitys;

namespace TestApi.Controllers
{
    [Route("/api/[controller]/[action]")]
    public class AuthController : Controller
    {
        IAuthRepository<User, UserRole> _auth;
        IRoleRepository<Role> _role;

        public AuthController(IAuthRepository<User, UserRole> auth,
        IRoleRepository<Role> role)
        {
            _auth = auth;
            _role = role;
        }
        public IActionResult Index()
        {
            return View();
        }
        public LoginResult Login()
        {
           return _auth.Login("joha", "123456").Result;

        }
        public object Register()
        {

           var s= _auth.RegisterAsync(new User() { UserName = "joha", PasswordHash = "123456" }).Result; ;
            return s;
        }
        public object AddRole()
        {
            _role.Add(new Role() { Name = "role" });
            return null;
        }

    }
}
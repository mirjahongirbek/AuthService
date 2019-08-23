using System;

namespace TestAuth
{
    class Program
    {
        static void Main(string[] args)
        {
          
                                   
        }
    }
    public class UserRoles : AuthService.Interfaces.Models.UserRole
    {

    }
    public class User : AuthService.Interfaces.Models.User<UserRoles>
    {
        public string CompanyID { get; set; }
    }
}

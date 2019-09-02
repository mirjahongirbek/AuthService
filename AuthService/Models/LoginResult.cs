using System.Collections.Generic;

namespace AuthService.Models
{
    public class LoginResult
    {
        public LoginResult()
        {
            Roles = new List<string>();

        }
        public string AccessToken { get; set; }
        public string UserName { get; set; }
        public string RefreshToken { get; set; }
       public List<string> Roles { get; set; }
    }
}


using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace AuthService
{
    public static class AuthOptions
    {
        public static string ISSUER { get; set; } = "MyAuthServer"; // издатель токена
        public static string AUDIENCE { get; set; } ="http://localhost:2600/"; // потребитель токена
        private static string KEY { get; set; } ="mysupersecret_secretkey!123"; // ключ для шифрации
        public static int LIFETIME { get; set; }= 2000; // время жизни токена - 1 минута
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
        public static bool CheckDeviceId { get; set; } = true;
        public static TimeSpan Otp { get; set; } = TimeSpan.FromSeconds(180);
    }
}

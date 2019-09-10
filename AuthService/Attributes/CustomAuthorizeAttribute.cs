using Microsoft.AspNetCore.Mvc;
using System;

namespace AuthService.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class CustomAuthorizeAttribute : TypeFilterAttribute
    {
        public CustomAuthorizeAttribute(
            string Roles = "",
            string MethodName = "",
            string UserName = "",
            int Position = 0)
            : base(typeof(AuthorizeActionFilter))
        {
            Arguments = new object[] { Roles, MethodName, UserName, Position };
        }
    }
    public class TokenAttribute : Attribute
    {
        public string Name { get; set; }
    }

}

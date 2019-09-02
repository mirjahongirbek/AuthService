using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthService.Attributes
{
    public class AuthorizeActionFilter : IAsyncActionFilter
    {
        private string _methodName;
        private string _userName;
        private int _position;
        private List<string> _roles;

        public AuthorizeActionFilter(string Roles = "",
            string MethodName = "",
            string UserName = "",
            int Position = 0)
        {
            _methodName = MethodName.ToLower();
            _userName = UserName.ToLower();
            _position = Position;
            _roles = new List<string>();
            foreach (var i in Roles.Split(","))
            {
                _roles.Add(i.ToLower().Trim());
            }
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
           var claims= context.HttpContext.User.Claims;
            
            if (_roles.Count() > 0)
            {
                foreach(var i in _roles)
                {
                    if(claims.FirstOrDefault(m => m.Value.ToLower() == i.ToLower())!= null){
                        break;
                    }
                }                
            }
            if (!string.IsNullOrEmpty(_methodName))
            {
               if( claims.FirstOrDefault(m => m.Type.ToLower() == "methodName" && m.Value.ToLower() == _methodName.ToLower())== null)
                {

                }
            }
            if (_position != 0)
            {
                if(claims.FirstOrDefault(m=>m.Type.ToLower()=="position" && m.Value.ToLower()== _position.ToString())== null)
                {

                }
            }
            if (!string.IsNullOrEmpty(_userName))
            {
                
            }

        }
    }

}

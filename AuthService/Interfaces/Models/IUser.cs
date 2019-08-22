using RepositoryCore.Interfaces;
using System.Collections.Generic;

namespace AuthService.Interfaces.Models
{
    public  interface IUser<TUserRole> : IEntity<int>
        where TUserRole:class, IUserRole
    {
       
        string UserName { get; set; }
        string Email { get; set; }
        string Salt { get; set; }
        string Password { get; set; }
        int Position { get; set; }
        ICollection<TUserRole> UserRoles { get; set; }
    }

}

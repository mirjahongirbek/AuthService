using RepositoryCore.Interfaces;
using System.Collections.Generic;

namespace AuthService.Interfaces.Models
{
    public  class User<TUserRole, TKey> : IEntity<int>
        where TUserRole: UserRole
    {
       public int Id { get; set; }
       public string UserName { get; set; }
       public string Email { get; set; }
       public string Salt { get; set; }
       public string Password { get; set; }
       public int Position { get; set; }
       public ICollection<TUserRole> UserRoles { get; set; }
    }

}

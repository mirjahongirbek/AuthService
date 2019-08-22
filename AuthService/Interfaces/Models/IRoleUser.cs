using RepositoryCore.Interfaces;

namespace AuthService.Interfaces.Models
{
    public class UserRole:IEntity<int>
    {
        public int Id { get; set; }
     public   int UserId { get; set; }
     public   User<this,int> User { get; set;}
     public   int RoleId { get; set; }
     public   Role<int> Role { get; set; }
    }


}

using RepositoryCore.Interfaces;

namespace AuthService.Interfaces.Models
{
    public interface IUserRole<TUser, TRole>:IEntity<int>
        where TUser: IUser<>
        where TRole: IRole
    {
      
        int UserId { get; set; }
        TUser User { get; set;}
        int RoleId { get; set; }
        IRole Role { get; set; }
    }


}

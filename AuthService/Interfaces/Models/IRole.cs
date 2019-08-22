using AuthService.Enum;
using RepositoryCore.Interfaces;


namespace AuthService.Interfaces.Models
{
    public interface IRole : IEntity<int>
    {
       
        string Name { get; set; }
        int Position { get; set; }
        string Description { get; set; }
        RoleEnum Roles { get; set; }
    }


}

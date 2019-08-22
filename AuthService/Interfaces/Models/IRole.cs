using AuthService.Enum;
using RepositoryCore.Interfaces;


namespace AuthService.Interfaces.Models
{
    public class Role<TKey> : IEntity<TKey>
    {
        public TKey Id { get; set; }
       public string Name { get; set; }
       public int Position { get; set; }
       public string Description { get; set; }
       public RoleEnum Roles { get; set; }
    }


}

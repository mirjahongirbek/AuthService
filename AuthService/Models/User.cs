using RepositoryCore.Models.Auth;

namespace AuthService.Models
{
    public  class EntityUser<TUserRole> : User<TUserRole,int>
        where TUserRole: EntityUserRole
    {
    }

}


namespace AuthService.Enum
{
    public enum PermissionEnum
    {
        Admin = 0,
        Moderator,
        Create,
        Read,
        Update,
        Delete,
        All,
        Edit
    }
    public enum UserStatus
    {
        NotActivated,
        Active,
        Deleted,
        Blocked,
        IsUserClient,
        SendOtp,
        SentOtpNewDevice
    }
}

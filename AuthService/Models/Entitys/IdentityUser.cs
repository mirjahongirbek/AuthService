using AuthService.Enum;
using RepositoryCore.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace AuthService.Models
{
    /// <summary>
    /// Represents a user in the identity system
    /// </summary>
    /// <typeparam name="TKey">The type used for the primary key for the user.</typeparam>
    public partial class IdentityUser:IEntity<int>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IdentityUser{TKey}"/>.
        /// </summary>
        public IdentityUser() { }
        /// <summary>
        /// Initializes a new instance of <see cref="IdentityUser{TKey}"/>.
        /// </summary>
        /// <param name="userName">The user name.</param>
        public IdentityUser(string userName) : this()
        {
            UserName = userName;
        }
        /// <summary>
        /// Gets or sets the primary key for this user.
        /// </summary>
        [Column("id")]
        public virtual int Id { get; set; }
        /// <summary>
        /// Gets or sets the user name for this user.
        /// </summary>
        [Column("user_name")]
        public virtual string UserName { get; set; }

        /// <summary>
        /// Gets or sets the normalized user name for this user.
        /// </summary>
        [NotMapped]
        public virtual string NormalizedUserName { get { return UserName.ToLower(); } }

        /// <summary>
        /// Gets or sets the email address for this user.
        /// </summary>
        [Column("email")]
        public virtual string Email { get; set; }
        [Column("last_otp")]
        public virtual string LastOtp { get; set; }
        [Column("last_otp_date")]
        public virtual DateTime LastOtpDate { get; set; }

        /// <summary>
        /// Gets or sets the normalized email address for this user.
        /// </summary>
        [NotMapped]
        public virtual string NormalizedEmail { get { return Email.ToLower(); } }

        /// <summary>
        /// Gets or sets a flag indicating if a user has confirmed their email address.
        /// </summary>
        /// <value>True if the email address has been confirmed, otherwise false.</value>
        [Column("confirm")]
        public virtual bool Confirmed { get; set; }
        [Column("user_status")]
        public virtual UserStatus UserStatus { get; set; }
        /// <summary>
        /// Gets or sets a salted and hashed representation of the password for this user.
        /// </summary>
        [IgnoreDataMember]
        [Column("password")]
        public virtual string Password { get; set; }
        [IgnoreDataMember]
        [Column("devices")]
        public string Devices { get; set; }
        [NotMapped]
        public List<string> DeviceList { get {
               var devices=Devices.Split(",");
                return devices.ToList();
            } }
        /// <summary>
        /// A random value that must change whenever a users credentials change (password changed, login removed)
        /// </summary>
        [Column("security_stamp")]
        public virtual string SecurityStamp { get; set; }

        /// <summary>
        /// A random value that must change whenever a user is persisted to the store
        /// </summary>
        [Column("concurrency_stamp")]
        public virtual string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets a telephone number for the user.
        /// </summary>
        [Column("phone_number")]
        public virtual string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if a user has confirmed their telephone address.
        /// </summary>
        /// <value>True if the telephone number has been confirmed, otherwise false.</value>
        [Column("phone_confirm")]
        public virtual bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if two factor authentication is enabled for this user.
        /// </summary>
        /// <value>True if 2fa is enabled, otherwise false.</value>
        [Column("two_factor_enabled")]
        public virtual bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in UTC, when any user lockout ends.
        /// </summary>
        /// <remarks>
        /// A value in the past means the user is not locked out.
        /// </remarks>
        [Column("lockout_end")]
        public virtual DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if the user could be locked out.
        /// </summary>
        /// <value>True if the user could be locked out, otherwise false.</value>
        [Column("lockout_enabled")]
        public virtual bool LockoutEnabled { get; set; }

        /// <summary>
        /// Gets or sets the number of failed login attempts for the current user.
        /// </summary>
        [Column("access_failed_count")]
        public virtual int AccessFailedCount { get; set; }

        /// <summary>
        /// Date when User Last Login In Site
        /// </summary>
        [Column("last_login_date")]
        public virtual DateTime LastLoginDate { get; set; }

        /// <summary>
        /// User Last Token
        /// </summary>
        [Column("token")]
        public virtual string Token { get; set; }

        /// <summary>
        ///  User RefResh Token
        /// </summary>
        [Column("refresh_token")]
        public virtual string RefreshToken { get; set; }

        /// <summary>
        /// User Position Jet JWt Token
        /// </summary>
        [Column("position")]
        public virtual int Position { get; set; }


        /// <summary>
        /// Returns the username for this user.
        /// </summary>
        public override string ToString()
            => UserName;
    }
    /// <summary>
    /// Identity User Functions
    /// </summary>
    public partial class IdentityUser
    {
        public bool CheckDevice(string deviceId)
        {
            if (string.IsNullOrEmpty(DeviceList.FirstOrDefault(m => m == deviceId))){
                return false;
            }
            return true;
        }
        public void AddDeviceId(string deviceId)
        {
            Devices += "," + deviceId;
        }
        
    }    

}

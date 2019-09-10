using RepositoryCore.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models
{
    public class IdentityUserRole:IEntity<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the primary key of the user that is linked to a role.
        /// </summary>
        [Column("user_id")]
        public virtual int UserId { get; set; }

        /// <summary>
        /// Gets or sets the primary key of the role that is linked to the user.
        /// </summary>
        [Column("role_id")]
        public virtual int RoleId { get; set; }
    }
    public class LoginResult
    {
        public LoginResult()
        {
            Roles = new List<string>();

        }
        public string AccessToken { get; set; }
        public string UserName { get; set; }
        public string RefreshToken { get; set; }
        public List<string> Roles { get; set; }
        public int MaxPosition { get; set; }
        public List<string> Actions { get; set; }
    }


}

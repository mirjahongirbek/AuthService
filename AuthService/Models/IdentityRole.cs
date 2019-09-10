using RepositoryCore.Enums.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthService.Models
{
    /// <summary>
    /// Represents a role in the identity system
    /// </summary>
    /// <typeparam name="TKey">The type used for the primary key for the role.</typeparam>
    public class IdentityRole
    {
        /// <summary>
        /// Initializes a new instance of <see cref="IdentityRole{TKey}"/>.
        /// </summary>
        public IdentityRole() { }

        /// <summary>
        /// Initializes a new instance of <see cref="IdentityRole{TKey}"/>.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        public IdentityRole(string roleName) : this()
        {
            Name = roleName;
        }

        /// <summary>
        /// Gets or sets the primary key for this role.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public virtual int Id { get; set; }

        /// <summary>
        /// Gets or sets the name for this role.
        /// </summary>
        [Column("name")]
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the normalized name for this role.
        /// </summary>
        [Column("normalize_name")]
        public virtual string NormalizedName { get; set; }

        /// <summary>
        /// A random value that should change whenever a role is persisted to the store
        /// </summary>

        public virtual string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Returns the name of the role.
        /// </summary>
        /// <returns>The name of the role.</returns>
        public override string ToString()
        {
            return Name;
        }
        /// <summary>
        /// Role Position If User Has many Position Jwt Set Just One
        /// </summary>
        [Column("position")]
        public int Position { get; set; }
        /// <summary>
        /// Role Description
        /// </summary>
        [Column("description")]
        public string Description { get; set; }
        /// <summary>
        /// Role Enum Set 
        /// </summary>
        [Column("roles")]
        public RoleEnum Roles { get; set; }
        /// <summary>
        /// Role Actions 
        /// </summary>
        [Column("actions")]
        public string Actions { get; set; }

        [NotMapped]
        public List<Action> ActionsList { get; }
    }


}

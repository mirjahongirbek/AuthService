using AuthService.Models;
using EntityRepository.Context;
using Microsoft.EntityFrameworkCore;

namespace TestApi.Entitys
{
    public class Db : DbContext, IDbContext
    {
        public Db(DbContextOptions<Db> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }        
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        public DbContext DataContext => this;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=blogging.db");
        }
    }
}

using Domain.Interface.Base;
using Domain.Permissions;
using Infrastructure.Seeders;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionType> PermissionTypes { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de las relaciones entre las entidades
            modelBuilder.Entity<Permission>()
                .HasOne(p => p.PermissionType)
                .WithMany(pt => pt.Permissions)
                .HasForeignKey(p => p.PermissionTypeId);
        }
    }
}

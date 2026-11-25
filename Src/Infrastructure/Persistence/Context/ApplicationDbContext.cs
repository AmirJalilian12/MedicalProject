using Domain.Entities.IdentityEntities;
using Domain.Entities.IdentityEntities.Medical;
using Domain.Entities.MedicalEntities.MedicineEntities;
using Domain.Entities.OrderEntities;
using Domain.Entities.ProductEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, string,
        IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductBrand> ProductBrands => Set<ProductBrand>();
        public DbSet<ProductType> ProductTypes => Set<ProductType>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<DeliveryMethod> DeliveryMethods => Set<DeliveryMethod>();
        public DbSet<Medicine> Medicines => Set<Medicine>();
        public DbSet<UserMedical> UserMedicals => Set<UserMedical>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });

                entity.HasOne(ur => ur.User)
                      .WithMany(u => u.UserRoles)
                      .HasForeignKey(ur => ur.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ur => ur.Role)
                      .WithMany(r => r.UserRoles)
                      .HasForeignKey(ur => ur.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<Product>().HasQueryFilter(x => x.IsDelete == false);
            modelBuilder.Entity<ProductType>().HasQueryFilter(x => x.IsDelete == false);
            modelBuilder.Entity<ProductBrand>().HasQueryFilter(x => x.IsDelete == false);
            modelBuilder.Entity<Address>().HasQueryFilter(x => x.IsDelete == false);
            modelBuilder.Entity<Order>().HasQueryFilter(x=>x.IsDelete == false);
            modelBuilder.Entity<DeliveryMethod>().HasQueryFilter(x=>x.IsDelete == false);
            modelBuilder.Entity<OrderItem>().HasQueryFilter(x=>x.IsDelete == false);
        }
    }
}

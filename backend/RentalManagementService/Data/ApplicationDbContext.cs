using Microsoft.EntityFrameworkCore;
using RentalManagementService.Models;
using System.Reflection.Emit;

namespace RentalManagementService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Property> Properties => Set<Property>();
        public DbSet<Unit> Units => Set<Unit>();
        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<Lease> Leases => Set<Lease>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<AppUser> Users => Set<AppUser>();
        public DbSet<MaintenanceRequest> MaintenanceRequests => Set<MaintenanceRequest>();
        public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);
            b.Entity<Unit>().HasOne(u => u.Property).WithMany(p => p.Units).HasForeignKey(u => u.PropertyId).OnDelete(DeleteBehavior.Cascade);
            b.Entity<Lease>().HasIndex(l => new { l.UnitId, l.Status });
            b.Entity<AppUser>().HasIndex(u => u.Email).IsUnique();
            b.Entity<PasswordResetToken>()
              .HasOne(p => p.User)
              .WithMany()
              .HasForeignKey(p => p.UserId)
              .OnDelete(DeleteBehavior.Cascade);

            b.Entity<PasswordResetToken>()
                      .HasIndex(p => p.TokenHash)
                      .IsUnique();
        }
    }
    public static class DbSeeder
    {
        public static void Seed(ApplicationDbContext db)
        {
            if (!db.Properties.Any())
            {
                var prop = new Property { Name = "Green Meadows", Address = "BTM Layout", City = "Bengaluru", State = "KA", Zip = "560076" };
                db.Properties.Add(prop); db.SaveChanges();
                db.Units.AddRange(
                  new Unit { PropertyId = prop.Id, UnitNumber = "A-101", Bedrooms = 2, Bathrooms = 2, RentAmount = 25000, Status = "Vacant" },
                  new Unit { PropertyId = prop.Id, UnitNumber = "A-102", Bedrooms = 3, Bathrooms = 2, RentAmount = 32000, Status = "Vacant" }
                );
                db.SaveChanges();
            }
            if (!db.Users.Any())
            {
                var admin = new AppUser { Email = "admin@local", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"), Role = "Admin" };
                db.Users.Add(admin); db.SaveChanges();
            }
        }
    }
}

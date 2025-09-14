// Models/AppUser.cs  (you already have this)
namespace RentalManagementService.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "Admin";  // Admin | Manager | Viewer
        public string SecurityStamp { get; set; } = Guid.NewGuid().ToString("N"); // invalidates JWTs when changed
        public int AccessFailedCount { get; set; }
        public DateTime? LockoutEndUtc { get; set; }
        public bool LockoutEnabled { get; set; } = true;
        public string Name { get; set; } = "";
    }
}

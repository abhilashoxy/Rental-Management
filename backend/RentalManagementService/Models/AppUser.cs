// Models/AppUser.cs  (you already have this)
namespace RentalManagementService.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "Admin";  // Admin | Manager | Viewer
    }
}

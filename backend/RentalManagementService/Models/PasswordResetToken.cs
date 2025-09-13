// Models/PasswordResetToken.cs
namespace RentalManagementService.Models
{
    public class PasswordResetToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TokenHash { get; set; } = "";  // store hash only
        public DateTime ExpiresAtUtc { get; set; }
        public bool Used { get; set; }

        public AppUser User { get; set; } = null!;
    }
}

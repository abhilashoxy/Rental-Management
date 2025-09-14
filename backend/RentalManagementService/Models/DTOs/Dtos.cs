namespace RentalManagementService.Models.DTOs
{
    public record PropertyListDto(int Id, string Name, string Address, int UnitCount);
    public record UnitDto(int Id, string UnitNumber, int Bedrooms, int Bathrooms, decimal RentAmount, string Status);
    public record TenantDto(int Id, string FirsName,string LastName, string Email);
    public record LeaseDto(int Id, int UnitId, int TenantId, DateTime StartDate, DateTime EndDate, decimal MonthlyRent, decimal Deposit, string Status);
    public record RegisterRequest(string Email, string Password, string Role);  // Role: Admin|Manager|Viewer
    public record LoginRequest(string Email, string Password);
    public record AuthResponse(string Token, string Email, string Role);
    public record ForgotPasswordRequest(string Email);
    public record ForgotPasswordResponse(string Message, string? DevToken); // DevToken returned only in dev
    public record ResetPasswordRequest(string Token, string NewPassword);
    public record MeDto(int Id, string Email, string? Name, string Role);
    public record UpdateProfileDto(string? Name);
    public record ChangePasswordDto(string CurrentPassword, string NewPassword);
    public record UpdateProfileResult(MeDto User, string? NewJwt); // NewJwt if claims changed
}


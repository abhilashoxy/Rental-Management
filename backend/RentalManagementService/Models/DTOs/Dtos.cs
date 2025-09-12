namespace RentalManagementService.Models.DTOs
{
    public record PropertyListDto(int Id, string Name, string Address, int UnitCount);
    public record UnitDto(int Id, string UnitNumber, int Bedrooms, int Bathrooms, decimal RentAmount, string Status);
    public record TenantDto(int Id, string FirsName,string LastName, string Email);
    public record LeaseDto(int Id, int UnitId, int TenantId, DateTime StartDate, DateTime EndDate, decimal MonthlyRent, decimal Deposit, string Status);
    public record RegisterRequest(string Email, string Password, string Role);  // Role: Admin|Manager|Viewer
    public record LoginRequest(string Email, string Password);
    public record AuthResponse(string Token, string Email, string Role);
}


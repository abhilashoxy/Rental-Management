namespace RentalManagementService.Models
{
    public record DashboardSummaryDto(
     int Properties,
     int Units,
     int Tenants,
     int OccupiedUnits,
     double OccupancyRatePercent,
     int DueThisMonth,
     int Overdue);
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalManagementService.Data;
using RentalManagementService.Models;

namespace RentalManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public DashboardController(ApplicationDbContext db) => _db = db;

        [HttpGet("summary")]
        public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
        {
            var properties = await _db.Properties.CountAsync();
            var units = await _db.Units.CountAsync();
            var tenants = await _db.Tenants.CountAsync();

            var occupiedUnits = await _db.Leases
                .Where(l => l.Status == "Active")
                .Select(l => l.UnitId)
                .Distinct()
                .CountAsync();

            var occupancyRate = units == 0 ? 0 : Math.Round(100.0 * occupiedUnits / units, 2);

            // placeholders until invoices/rents are added
            int dueThisMonth = 0;
            int overdue = 0;

            return Ok(new DashboardSummaryDto(
                properties,
                units,
                tenants,
                occupiedUnits,
                occupancyRate,
                dueThisMonth,
                overdue
            ));
        }
    }
}

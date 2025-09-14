using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalManagementService.Data;

using RentalManagementService.Models;
using RentalManagementService.Models.DTOs;

namespace RentalManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LeasesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public LeasesController(ApplicationDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaseDto>>> GetAll()
        {
            var data = await _db.Leases
                .AsNoTracking()
                .Select(l => new LeaseDto(l.Id, l.UnitId, l.TenantId, l.StartDate, l.EndDate, l.MonthlyRent, l.Deposit, l.Status))
                .ToListAsync();
            return Ok(data);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(Lease lease)
        {
            _db.Leases.Add(lease);
            await _db.SaveChangesAsync();
            return Ok(lease);
        }
    }
}

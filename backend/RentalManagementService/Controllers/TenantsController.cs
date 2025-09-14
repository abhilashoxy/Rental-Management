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
    public class TenantsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public TenantsController(ApplicationDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TenantDto>>> GetAll()
        {
            var data = await _db.Tenants
                .AsNoTracking()
                .Select(t => new TenantDto(t.Id, t.FirstName,t.LastName, t.Email))
                .ToListAsync();
            return Ok(data);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(Tenant tenant)
        {
            _db.Tenants.Add(tenant);
            await _db.SaveChangesAsync();
            return Ok(tenant);
        }
    }
}

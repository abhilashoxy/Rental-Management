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
    public class UnitsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public UnitsController(ApplicationDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UnitDto>>> GetAll()
        {
            var data = await _db.Units
                .AsNoTracking()
                .Select(u => new UnitDto(u.Id, u.UnitNumber, u.Bedrooms, u.Bathrooms, u.RentAmount, u.Status))
                .ToListAsync();
            return Ok(data);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(Unit u)
        {
            _db.Units.Add(u);
            await _db.SaveChangesAsync();
            return Ok(u);
        }

        [HttpPatch("{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, Unit input)
        {
            var u = await _db.Units.FindAsync(id);
            if (u is null) return NotFound();

            u.UnitNumber = input.UnitNumber;
            u.Bedrooms = input.Bedrooms;
            u.Bathrooms = input.Bathrooms;
            u.RentAmount = input.RentAmount;
            u.Status = input.Status;

            await _db.SaveChangesAsync();
            return Ok(u);
        }
    }
}

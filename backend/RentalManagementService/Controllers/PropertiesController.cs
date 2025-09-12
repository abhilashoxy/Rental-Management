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
    public class PropertiesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public PropertiesController(ApplicationDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PropertyListDto>>> GetAll()
        {
            var data = await _db.Properties
                .AsNoTracking()
                .Select(p => new PropertyListDto(p.Id, p.Name, p.Address, p.Units.Count))
                .ToListAsync();
            return Ok(data);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(Property property)
        {
            _db.Properties.Add(property);
            await _db.SaveChangesAsync();
            return Ok(property);
        }
    }
}

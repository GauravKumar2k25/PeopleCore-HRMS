using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleCore.Api.Data;
using PeopleCore.Api.DTOs;
using PeopleCore.Api.Models;
namespace PeopleCore.Api.Controllers;
[ApiController, Route("api/[controller]"), Authorize]
public class EmployeesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        page = Math.Max(page, 1); pageSize = Math.Clamp(pageSize, 1, 100);
        var query = db.Employees.Include(e => e.Department).AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(e => e.FirstName.Contains(search) || e.LastName.Contains(search) || e.Email.Contains(search) || e.JobTitle.Contains(search));
        var total = await query.CountAsync(); var items = await query.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return Ok(new { page, pageSize, total, items });
    }
    [HttpGet("{id:int}")] public async Task<IActionResult> Get(int id) { var e = await db.Employees.Include(x => x.Department).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id); return e is null ? NotFound() : Ok(e); }
    [HttpPost, Authorize(Roles = "Admin,HR")] public async Task<IActionResult> Create(EmployeeCreateRequest r)
    {
        var email = r.Email.Trim().ToLowerInvariant(); if (await db.Employees.AnyAsync(e => e.Email == email)) return Conflict(new { message = "Employee email already exists." });
        if (r.DepartmentId is not null && !await db.Departments.AnyAsync(d => d.Id == r.DepartmentId)) return BadRequest(new { message = "Department does not exist." });
        var e = new Employee { FirstName = r.FirstName.Trim(), LastName = r.LastName.Trim(), Email = email, Phone = r.Phone, JobTitle = r.JobTitle.Trim(), DepartmentId = r.DepartmentId, DateJoined = r.DateJoined.Date };
        db.Employees.Add(e); await db.SaveChangesAsync(); return CreatedAtAction(nameof(Get), new { id = e.Id }, e);
    }
    [HttpPut("{id:int}"), Authorize(Roles = "Admin,HR")] public async Task<IActionResult> Update(int id, EmployeeUpdateRequest r)
    {
        var e = await db.Employees.FindAsync(id); if (e is null) return NotFound(); var email = r.Email.Trim().ToLowerInvariant();
        if (await db.Employees.AnyAsync(x => x.Id != id && x.Email == email)) return Conflict(new { message = "Employee email already exists." });
        if (r.DepartmentId is not null && !await db.Departments.AnyAsync(d => d.Id == r.DepartmentId)) return BadRequest(new { message = "Department does not exist." });
        e.FirstName=r.FirstName.Trim(); e.LastName=r.LastName.Trim(); e.Email=email; e.Phone=r.Phone; e.JobTitle=r.JobTitle.Trim(); e.DepartmentId=r.DepartmentId; e.DateJoined=r.DateJoined.Date; e.IsActive=r.IsActive; await db.SaveChangesAsync(); return NoContent();
    }
    [HttpDelete("{id:int}"), Authorize(Roles = "Admin,HR")] public async Task<IActionResult> Delete(int id) { var e = await db.Employees.FindAsync(id); if (e is null) return NotFound(); e.IsActive = false; await db.SaveChangesAsync(); return NoContent(); }
}

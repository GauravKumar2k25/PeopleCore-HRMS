using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleCore.Api.Data;
using PeopleCore.Api.DTOs;
using PeopleCore.Api.Models;
namespace PeopleCore.Api.Controllers;
[ApiController, Route("api/[controller]"), Authorize]
public class DepartmentsController(AppDbContext db) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> GetAll() => Ok(await db.Departments.AsNoTracking().OrderBy(d => d.Name).Select(d => new { d.Id, d.Name, d.Description, EmployeeCount = d.Employees.Count }).ToListAsync());
    [HttpPost, Authorize(Roles="Admin,HR")] public async Task<IActionResult> Create(DepartmentRequest r) { var name=r.Name.Trim(); if(await db.Departments.AnyAsync(d=>d.Name==name)) return Conflict(new {message="Department already exists."}); var d=new Department{Name=name,Description=r.Description}; db.Departments.Add(d); await db.SaveChangesAsync(); return CreatedAtAction(nameof(GetAll), new {id=d.Id}, d); }
    [HttpDelete("{id:int}"), Authorize(Roles="Admin,HR")] public async Task<IActionResult> Delete(int id) { var d=await db.Departments.FindAsync(id); if(d is null)return NotFound(); if(await db.Employees.AnyAsync(e=>e.DepartmentId==id))return Conflict(new {message="Move employees to another department before deleting this department."}); db.Departments.Remove(d); await db.SaveChangesAsync(); return NoContent(); }
}

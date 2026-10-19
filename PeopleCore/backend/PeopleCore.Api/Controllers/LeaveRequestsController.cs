using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleCore.Api.Data;
using PeopleCore.Api.DTOs;
using PeopleCore.Api.Models;
namespace PeopleCore.Api.Controllers;
[ApiController, Route("api/[controller]"), Authorize]
public class LeaveRequestsController(AppDbContext db) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> GetAll([FromQuery] string? status) { var q=db.LeaveRequests.Include(x=>x.Employee).AsNoTracking(); if(Enum.TryParse<LeaveStatus>(status,true,out var parsed)) q=q.Where(x=>x.Status==parsed); return Ok(await q.OrderByDescending(x=>x.CreatedAtUtc).Select(x=>new {x.Id,x.EmployeeId,EmployeeName=x.Employee!.FirstName+" "+x.Employee.LastName,x.Reason,x.StartDate,x.EndDate,x.Status,x.CreatedAtUtc,x.ReviewedBy}).ToListAsync()); }
    [HttpPost] public async Task<IActionResult> Create(LeaveCreateRequest r) { if(r.EndDate.Date<r.StartDate.Date)return BadRequest(new {message="End date cannot be before start date."}); if(!await db.Employees.AnyAsync(e=>e.Id==r.EmployeeId && e.IsActive))return BadRequest(new {message="Active employee not found."}); var item=new LeaveRequest{EmployeeId=r.EmployeeId,Reason=r.Reason.Trim(),StartDate=r.StartDate.Date,EndDate=r.EndDate.Date}; db.LeaveRequests.Add(item); await db.SaveChangesAsync(); return CreatedAtAction(nameof(GetAll),new{id=item.Id},item); }
    [HttpPatch("{id:int}/review"), Authorize(Roles="Admin,HR,Manager")] public async Task<IActionResult> Review(int id, LeaveReviewRequest r) { var item=await db.LeaveRequests.FindAsync(id); if(item is null)return NotFound(); if(item.Status!=LeaveStatus.Pending)return Conflict(new {message="Only pending requests can be reviewed."}); item.Status=r.Approve?LeaveStatus.Approved:LeaveStatus.Rejected; item.ReviewedBy=User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name; await db.SaveChangesAsync(); return Ok(new {item.Id,item.Status,item.ReviewedBy}); }
}

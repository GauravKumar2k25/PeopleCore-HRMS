using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleCore.Api.Data;
using PeopleCore.Api.Models;
namespace PeopleCore.Api.Controllers;
[ApiController, Route("api/[controller]"), Authorize(Roles="Admin,HR,Manager")]
public class DashboardController(AppDbContext db) : ControllerBase
{
    [HttpGet("summary")] public async Task<IActionResult> Summary() => Ok(new { totalEmployees=await db.Employees.CountAsync(e=>e.IsActive), totalDepartments=await db.Departments.CountAsync(), pendingLeaves=await db.LeaveRequests.CountAsync(l=>l.Status==LeaveStatus.Pending), approvedLeaves=await db.LeaveRequests.CountAsync(l=>l.Status==LeaveStatus.Approved), employeesByDepartment=await db.Departments.Select(d=>new {d.Name,count=d.Employees.Count(e=>e.IsActive)}).ToListAsync() });
}

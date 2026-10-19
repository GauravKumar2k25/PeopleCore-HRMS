using System.ComponentModel.DataAnnotations;
namespace PeopleCore.Api.Models;
public enum LeaveStatus { Pending, Approved, Rejected }
public class LeaveRequest { public int Id { get; set; } public int EmployeeId { get; set; } public Employee? Employee { get; set; } [Required, MaxLength(500)] public string Reason { get; set; } = ""; public DateTime StartDate { get; set; } public DateTime EndDate { get; set; } public LeaveStatus Status { get; set; } = LeaveStatus.Pending; public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow; public string? ReviewedBy { get; set; } }

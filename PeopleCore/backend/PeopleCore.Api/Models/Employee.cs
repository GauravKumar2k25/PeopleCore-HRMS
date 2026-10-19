using System.ComponentModel.DataAnnotations;
namespace PeopleCore.Api.Models;
public class Employee
{
    public int Id { get; set; }
    [Required, MaxLength(80)] public string FirstName { get; set; } = "";
    [Required, MaxLength(80)] public string LastName { get; set; } = "";
    [Required, EmailAddress, MaxLength(180)] public string Email { get; set; } = "";
    [MaxLength(30)] public string Phone { get; set; } = "";
    [Required, MaxLength(100)] public string JobTitle { get; set; } = "";
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public DateTime DateJoined { get; set; } = DateTime.UtcNow.Date;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

using System.ComponentModel.DataAnnotations;
namespace PeopleCore.Api.Models;
public class AppUser { public int Id { get; set; } [Required, MaxLength(100)] public string FullName { get; set; } = ""; [Required, EmailAddress, MaxLength(180)] public string Email { get; set; } = ""; [Required] public string PasswordHash { get; set; } = ""; [Required] public string Role { get; set; } = "Employee"; public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow; }

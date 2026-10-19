using System.ComponentModel.DataAnnotations;
namespace PeopleCore.Api.Models;
public class Department { public int Id { get; set; } [Required, MaxLength(100)] public string Name { get; set; } = ""; [MaxLength(250)] public string? Description { get; set; } public ICollection<Employee> Employees { get; set; } = new List<Employee>(); }

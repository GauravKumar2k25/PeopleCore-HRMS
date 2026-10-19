using System.ComponentModel.DataAnnotations;
namespace PeopleCore.Api.DTOs;
public record EmployeeCreateRequest([Required,MaxLength(80)] string FirstName,[Required,MaxLength(80)] string LastName,[Required,EmailAddress] string Email,[MaxLength(30)] string Phone,[Required,MaxLength(100)] string JobTitle,int? DepartmentId,DateTime DateJoined);
public record EmployeeUpdateRequest([Required,MaxLength(80)] string FirstName,[Required,MaxLength(80)] string LastName,[Required,EmailAddress] string Email,[MaxLength(30)] string Phone,[Required,MaxLength(100)] string JobTitle,int? DepartmentId,DateTime DateJoined,bool IsActive);
public record DepartmentRequest([Required,MaxLength(100)] string Name,[MaxLength(250)] string? Description);
public record LeaveCreateRequest(int EmployeeId,[Required,MaxLength(500)] string Reason,DateTime StartDate,DateTime EndDate);
public record LeaveReviewRequest(bool Approve);

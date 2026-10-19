using System.ComponentModel.DataAnnotations;
namespace PeopleCore.Api.DTOs;
public record RegisterRequest([Required,MaxLength(100)] string FullName,[Required,EmailAddress] string Email,[Required,MinLength(8)] string Password);
public record LoginRequest([Required,EmailAddress] string Email,[Required] string Password);
public record AuthResponse(string Token,string FullName,string Email,string Role);

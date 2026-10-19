using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeopleCore.Api.Data;
using PeopleCore.Api.DTOs;
using PeopleCore.Api.Models;
using PeopleCore.Api.Services;
namespace PeopleCore.Api.Controllers;
[ApiController, Route("api/[controller]")]
public class AuthController(AppDbContext db, TokenService tokens) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(u => u.Email == email)) return Conflict(new { message = "An account with this email already exists." });
        var user = new AppUser { FullName = request.FullName.Trim(), Email = email, PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password), Role = "Employee" };
        db.Users.Add(user); await db.SaveChangesAsync();
        return Ok(new AuthResponse(tokens.CreateToken(user), user.FullName, user.Email, user.Role));
    }
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant(); var user = await db.Users.SingleOrDefaultAsync(u => u.Email == email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) return Unauthorized(new { message = "Invalid email or password." });
        return Ok(new AuthResponse(tokens.CreateToken(user), user.FullName, user.Email, user.Role));
    }
}

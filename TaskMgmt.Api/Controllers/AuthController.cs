using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TaskMgmt.Api.Data;
using TaskMgmt.Api.Dtos.Auth;
using TaskMgmt.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using TaskMgmt.Api.Services.Implementations;        // <-- for PasswordHasher
using TaskMgmt.Api.Domain.Entities;                 // <-- for User



namespace TaskMgmt.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IJwtTokenService _jwt;

    public AuthController(AppDbContext db, IJwtTokenService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest req)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == req.Username);
        if (user is null) return Unauthorized(new { message = "Invalid credentials" });

        using var hmac = new HMACSHA256(user.PasswordSalt);
        var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(req.Password));
        if (!computed.SequenceEqual(user.PasswordHash))
            return Unauthorized(new { message = "Invalid credentials" });

        var token = _jwt.CreateToken(user);
        return new AuthResponse
        {
            Token = token,
            UserId = user.Id,
            Username = user.Username,
            Name = user.Name,
            Role = user.Role
        };
    }
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest req)
    {
        if (await _db.Users.AnyAsync(u => u.Username == req.Username))
            return BadRequest(new { message = "Username already exists" });

        PasswordHasher.Create(req.Password, out var hash, out var salt);

        var user = new User
        {
            Username = req.Username,
            PasswordHash = hash,
            PasswordSalt = salt,
            Name = req.Name,
            Email = req.Email,
            Role = "Employee"
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = _jwt.CreateToken(user);
        return new AuthResponse { Token = token, UserId = user.Id, Username = user.Username, Name = user.Name, Role = user.Role };
    }
}

using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskMgmt.Api.Data;
using TaskMgmt.Api.Domain.Entities;
using TaskMgmt.Api.Dtos.Users;
using TaskMgmt.Api.Services.Implementations;

namespace TaskMgmt.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Manager")] // managers manage users
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public UsersController(AppDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserReadDto>>> GetAll()
        => await _db.Users.OrderByDescending(u => u.Id)
            .ProjectTo<UserReadDto>(_mapper.ConfigurationProvider).ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserReadDto>> Get(int id)
        => await _db.Users.Where(u => u.Id == id)
            .ProjectTo<UserReadDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync()
           is { } u ? u : NotFound();

    [HttpPost]
    public async Task<ActionResult<UserReadDto>> Create(UserCreateDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
            return BadRequest(new { message = "Username already exists" });

        PasswordHasher.Create(dto.Password, out var hash, out var salt);
        var u = _mapper.Map<User>(dto);
        u.PasswordHash = hash; u.PasswordSalt = salt;

        _db.Users.Add(u);
        await _db.SaveChangesAsync();

        var read = _mapper.Map<UserReadDto>(u);
        return CreatedAtAction(nameof(Get), new { id = u.Id }, read);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserReadDto>> Update(int id, UserUpdateDto dto)
    {
        var u = await _db.Users.FindAsync(id);
        if (u is null) return NotFound();

        u.Name = dto.Name; u.Email = dto.Email; u.Role = dto.Role;
        await _db.SaveChangesAsync();
        return _mapper.Map<UserReadDto>(u);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var u = await _db.Users.FindAsync(id);
        if (u is null) return NotFound();
        _db.Users.Remove(u);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskMgmt.Api.Data;
using TaskMgmt.Api.Domain.Entities;
using TaskMgmt.Api.Dtos.Projects;
using TaskMgmt.Api.Services.Interfaces;

namespace TaskMgmt.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _me;

    public ProjectsController(AppDbContext db, IMapper mapper, ICurrentUserService me)
    {
        _db = db; _mapper = mapper; _me = me;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectReadDto>>> GetAll()
        => await _db.Projects
            .OrderByDescending(p => p.Id)
            .ProjectTo<ProjectReadDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectReadDto>> GetById(int id)
    {
        var proj = await _db.Projects.FindAsync(id);
        return proj == null ? NotFound() : _mapper.Map<ProjectReadDto>(proj);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectReadDto>> Create(ProjectCreateDto dto)
    {
        if (_me.UserId is null) return Unauthorized();

        var entity = _mapper.Map<Project>(dto);
        entity.CreatedByUserId = _me.UserId.Value;

        _db.Projects.Add(entity);
        await _db.SaveChangesAsync();

        var read = _mapper.Map<ProjectReadDto>(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, read);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProjectReadDto>> Update(int id, ProjectCreateDto dto)
    {
        var entity = await _db.Projects.FindAsync(id);
        if (entity is null) return NotFound();

        entity.Name = dto.Name;
        entity.Description = dto.Description;

        await _db.SaveChangesAsync();
        return _mapper.Map<ProjectReadDto>(entity);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Projects.FindAsync(id);
        if (entity is null) return NotFound();

        _db.Projects.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

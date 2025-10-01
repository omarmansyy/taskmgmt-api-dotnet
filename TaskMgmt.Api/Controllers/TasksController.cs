using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskMgmt.Api.Data;
using TaskMgmt.Api.Domain.Entities;
using TaskMgmt.Api.Dtos.Tasks;
using TaskMgmt.Api.Services.Interfaces;

namespace TaskMgmt.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _me;

    public TasksController(AppDbContext db, IMapper mapper, ICurrentUserService me)
    {
        _db = db; _mapper = mapper; _me = me;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskReadDto>>> Query([FromQuery] TaskQueryParams q)
    {
        var tasks = _db.Tasks.AsQueryable();

        if (q.Status.HasValue) tasks = tasks.Where(t => t.Status == q.Status);
        if (q.Priority.HasValue) tasks = tasks.Where(t => t.Priority == q.Priority);
        if (q.ProjectId.HasValue) tasks = tasks.Where(t => t.ProjectId == q.ProjectId);
        if (q.AssignedToUserId.HasValue) tasks = tasks.Where(t => t.AssignedToUserId == q.AssignedToUserId);
        if (!string.IsNullOrWhiteSpace(q.Search))
            tasks = tasks.Where(t => t.Title.Contains(q.Search) || (t.Description ?? "").Contains(q.Search));

        var skip = (q.Page - 1) * q.PageSize;

        return await tasks
            .OrderByDescending(t => t.Id)
            .Skip(skip)
            .Take(q.PageSize)
            .ProjectTo<TaskReadDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskReadDto>> Get(int id)
    {
        var entity = await _db.Tasks.FindAsync(id);
        return entity is null ? NotFound() : _mapper.Map<TaskReadDto>(entity);
    }

    [HttpPost]
    public async Task<ActionResult<TaskReadDto>> Create(TaskCreateDto dto)
    {
        if (_me.UserId is null) return Unauthorized();

        var entity = _mapper.Map<TaskItem>(dto);
        entity.CreatedByUserId = _me.UserId.Value;

        _db.Tasks.Add(entity);
        await _db.SaveChangesAsync();

        var read = _mapper.Map<TaskReadDto>(entity);
        return CreatedAtAction(nameof(Get), new { id = entity.Id }, read);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskReadDto>> Update(int id, TaskUpdateDto dto)
    {
        var entity = await _db.Tasks.FindAsync(id);
        if (entity is null) return NotFound();

        _mapper.Map(dto, entity);
        await _db.SaveChangesAsync();
        return _mapper.Map<TaskReadDto>(entity);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.Tasks.FindAsync(id);
        if (entity is null) return NotFound();

        _db.Tasks.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
    [HttpPost("{id:int}/image")]
    [RequestSizeLimit(5_000_000)] // ~5 MB
    public async Task<ActionResult<TaskReadDto>> UploadImage(int id, IFormFile file, [FromServices] IWebHostEnvironment env)
    {
        var entity = await _db.Tasks.FindAsync(id);
        if (entity is null) return NotFound();
        if (file is null || file.Length == 0) return BadRequest(new { message = "No file" });
        if (!file.ContentType.StartsWith("image/")) return BadRequest(new { message = "Only image files allowed" });

        var uploads = Path.Combine(env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot"), "uploads");
        Directory.CreateDirectory(uploads);

        var ext = Path.GetExtension(file.FileName);
        var name = $"task_{id}_{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(uploads, name);

        await using var stream = System.IO.File.Create(fullPath);
        await file.CopyToAsync(stream);

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        entity.ImageUrl = $"{baseUrl}/uploads/{name}";
        await _db.SaveChangesAsync();

        return _mapper.Map<TaskReadDto>(entity);
    }

}

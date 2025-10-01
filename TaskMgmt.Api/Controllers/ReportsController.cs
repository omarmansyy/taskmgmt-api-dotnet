using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskMgmt.Api.Data;
using TaskMgmt.Api.Domain.Enums;

namespace TaskMgmt.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ReportsController(AppDbContext db) => _db = db;

    [HttpGet("tasks/summary")]
    public async Task<IActionResult> TaskSummary()
    {
        var now = DateTime.UtcNow;

        var byStatus = await _db.Tasks
            .GroupBy(t => t.Status)
            .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
            .ToListAsync();

        var byPriority = await _db.Tasks
            .GroupBy(t => t.Priority)
            .Select(g => new { Priority = g.Key.ToString(), Count = g.Count() })
            .ToListAsync();

        var overdue = await _db.Tasks
            .CountAsync(t => t.DueDate != null && t.DueDate < now && t.Status != TaskStatus.Done);

        return Ok(new { byStatus, byPriority, overdue });
    }
}

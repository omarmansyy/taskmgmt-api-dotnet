using Microsoft.EntityFrameworkCore;
using Quartz;
using TaskMgmt.Api.Data;
using TaskMgmt.Api.Domain.Enums;
using TaskMgmt.Api.Services.Interfaces;

namespace TaskMgmt.Api.Jobs;

public class OverdueTaskNotifierJob : IJob
{
    private readonly AppDbContext _db;
    private readonly IEmailService _email;

    public OverdueTaskNotifierJob(AppDbContext db, IEmailService email) { _db = db; _email = email; }

    public async Task Execute(IJobExecutionContext context)
    {
        var today = DateTime.UtcNow;
        var tasks = await _db.Tasks
            .Include(t => t.AssignedToUser)
            .Where(t => t.DueDate != null && t.DueDate < today && t.Status != TaskStatus.Done)
            .ToListAsync();

        foreach (var t in tasks.Where(t => t.AssignedToUser != null))
        {
            var to = t.AssignedToUser!.Email;
            var subject = $"Overdue Task: {t.Title}";
            var body = $"<p>The task <strong>{t.Title}</strong> (Project #{t.ProjectId}) is overdue.</p>" +
                       $"<p>Due: {t.DueDate:yyyy-MM-dd}</p>";
            await _email.SendAsync(to, subject, body);
        }
    }
}

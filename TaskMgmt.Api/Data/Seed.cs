using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TaskMgmt.Api.Domain.Entities;
using TaskMgmt.Api.Domain.Enums;

namespace TaskMgmt.Api.Data;

public static class Seed
{
    public static async Task RunAsync(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        if (await db.Users.AnyAsync()) return;

        void CreatePassword(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA256();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        CreatePassword("Pass@123", out var mHash, out var mSalt);
        var manager = new User { Username = "manager", Name = "Manager One", Email = "manager@example.com", Role = "Manager", PasswordHash = mHash, PasswordSalt = mSalt };

        CreatePassword("Pass@123", out var eHash, out var eSalt);
        var employee = new User { Username = "employee", Name = "Employee One", Email = "employee@example.com", Role = "Employee", PasswordHash = eHash, PasswordSalt = eSalt };

        db.Users.AddRange(manager, employee);

        var proj = new Project { Name = "Onboarding", Description = "Initial setup", CreatedByUser = manager };
        db.Projects.Add(proj);

        db.Tasks.AddRange(
            new TaskItem { Title = "Create repo", Project = proj, Priority = TaskPriority.High, Status = TaskState.Done, CreatedByUser = manager, AssignedToUser = manager, DueDate = DateTime.UtcNow.AddDays(-1) },
            new TaskItem { Title = "Write README", Project = proj, Priority = TaskPriority.Medium, Status = TaskState.InProgress, CreatedByUser = manager, AssignedToUser = employee, DueDate = DateTime.UtcNow.AddDays(2) }
        );

        await db.SaveChangesAsync();
    }
}

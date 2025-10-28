using Microsoft.EntityFrameworkCore;
using TodoWarrior.Api.Data;
using TodoWarrior.Api.Models;

namespace TodoWarrior.Api.Extensions
{
    public static class WebApplicationExtensions
    {
        public static void EnsureDatabaseMigrationAndSeedData(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Apply any pending migrations
            db.Database.Migrate();

            // Create DB if it does not exist
            db.Database.EnsureCreated();

            // If main table is empty, seed some sample tasks
            if (!db.TaskItems.Any())
            {
                var now = DateTime.UtcNow;
                var today = DateOnly.FromDateTime(now);
                var tasks = new List<TaskItem>
                {
                    // Today's tasks
                    new TaskItem { Guid = Guid.NewGuid(), Title = "Morning Standup", Description = "Team sync meeting", DueDate = today, IsDone = false, IsActive = true, ReminderAt = now.AddMinutes(10) },
                    new TaskItem { Guid = Guid.NewGuid(), Title = "Code Review PR #123", Description = "Review authentication changes", DueDate = today, IsDone = false, IsActive = true, ReminderAt = now.AddMinutes(30) },
                    new TaskItem { Guid = Guid.NewGuid(), Title = "Update Documentation", Description = "Document new API endpoints", DueDate = today, IsDone = true, IsActive = true, ReminderAt = null },
                    
                    // Tomorrow's tasks
                    new TaskItem { Guid = Guid.NewGuid(), Title = "Deploy to Staging", Description = "Deploy version 2.1.0", DueDate = today.AddDays(1), IsDone = false, IsActive = true, ReminderAt = now.AddHours(1) },
                    new TaskItem { Guid = Guid.NewGuid(), Title = "Client Demo", Description = "Present new features to stakeholders", DueDate = today.AddDays(1), IsDone = false, IsActive = true, ReminderAt = now.AddHours(20) },
                    new TaskItem { Guid = Guid.NewGuid(), Title = "Database Backup", Description = "Verify backup procedures", DueDate = today.AddDays(1), IsDone = false, IsActive = true, ReminderAt = now.AddHours(24) },
                    
                    // Day after tomorrow
                    new TaskItem { Guid = Guid.NewGuid(), Title = "Security Audit", Description = "Review security vulnerabilities", DueDate = today.AddDays(2), IsDone = false, IsActive = true, ReminderAt = now.AddDays(2) },
                    new TaskItem { Guid = Guid.NewGuid(), Title = "Performance Testing", Description = "Load test new endpoints", DueDate = today.AddDays(2), IsDone = false, IsActive = true, ReminderAt = null },
                    
                    // Next week
                    new TaskItem { Guid = Guid.NewGuid(), Title = "Sprint Planning", Description = "Plan Sprint 24", DueDate = today.AddDays(7), IsDone = false, IsActive = true, ReminderAt = now.AddDays(6) },
                    new TaskItem { Guid = Guid.NewGuid(), Title = "Update Dependencies", Description = "Upgrade NuGet packages", DueDate = today.AddDays(7), IsDone = false, IsActive = true, ReminderAt = null },
                    new TaskItem { Guid = Guid.NewGuid(), Title = "Team Retrospective", Description = "Sprint 23 retrospective", DueDate = today.AddDays(7), IsDone = false, IsActive = true, ReminderAt = now.AddDays(6).AddHours(2) },
                    
                    // Two weeks 
                    new TaskItem { Guid = Guid.NewGuid(), Title = "Production Release", Description = "Deploy v2.1.0 to production", DueDate = today.AddDays(14), IsDone = false, IsActive = true, ReminderAt = now.AddDays(13) },
                    new TaskItem { Guid = Guid.NewGuid(), Title = "Quarterly Review", Description = "Q4 goals review meeting", DueDate = today.AddDays(14), IsDone = false, IsActive = true, ReminderAt = null },
                    
                    // Yesterday (overdue)
                    new TaskItem { Guid = Guid.NewGuid(), Title = "Fix Critical Bug", Description = "Login timeout issue", DueDate = today.AddDays(-1), IsDone = false, IsActive = true, ReminderAt = null },
                    new TaskItem { Guid = Guid.NewGuid(), Title = "Weekly Report", Description = "Submit progress report", DueDate = today.AddDays(-1), IsDone = true, IsActive = true, ReminderAt = null },
                };

                db.TaskItems.AddRange(tasks);
                db.SaveChanges();
            }
        }
    }
}
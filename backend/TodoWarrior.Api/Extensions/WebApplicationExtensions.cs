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
                var tasks = new List<TaskItem>
                {
                    new TaskItem { Guid = Guid.NewGuid(), Title = "Seed Task 1", Description = "Seeded task", IsDone = false, IsActive = true, ReminderAt = now.AddMinutes(10) },
                    new TaskItem { Guid = Guid.NewGuid(), Title = "Seed Task 2", Description = "Seeded task", IsDone = false, IsActive = true, ReminderAt = now.AddMinutes(30) },
                    new TaskItem { Guid = Guid.NewGuid(), Title = "Seed Task 3", Description = "Seeded task", IsDone = true,  IsActive = true, ReminderAt = null },
                    new TaskItem { Guid = Guid.NewGuid(), Title = "Seed Task 4", Description = "Seeded task", IsDone = false, IsActive = true, ReminderAt = now.AddHours(1) },
                    new TaskItem { Guid = Guid.NewGuid(), Title = "Seed Task 5", Description = "Seeded task", IsDone = false, IsActive = true, ReminderAt = now.AddMinutes(5) },
                    new TaskItem { Guid = Guid.NewGuid(), Title = "Seed Task 6", Description = "Seeded task", IsDone = true,  IsActive = true, ReminderAt = null },
                    new TaskItem { Guid = Guid.NewGuid(), Title = "Seed Task 7", Description = "Seeded task", IsDone = false, IsActive = true, ReminderAt = now.AddDays(1) },
                    new TaskItem { Guid = Guid.NewGuid(), Title = "Seed Task 8", Description = "Seeded task", IsDone = false, IsActive = true, ReminderAt = now.AddMinutes(2) },
                    new TaskItem { Guid = Guid.NewGuid(), Title = "Seed Task 9", Description = "Seeded task", IsDone = false, IsActive = true, ReminderAt = null },
                    new TaskItem { Guid = Guid.NewGuid(), Title = "Seed Task 10",Description = "Seeded task", IsDone = false, IsActive = true, ReminderAt = now.AddHours(2) },
                };

                db.TaskItems.AddRange(tasks);
                db.SaveChanges();
            }
        }
    }
}
namespace TodoWarrior.Api.Models;
public class TaskItem
{
    public Guid Guid { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public bool IsDone { get; set; } = false;
    public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateTimeOffset? ReminderAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    public void Touch() => UpdatedAt = DateTime.UtcNow;
}
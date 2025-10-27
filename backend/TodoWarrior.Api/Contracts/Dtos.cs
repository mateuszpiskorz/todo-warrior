namespace TodoWarrior.Api.Contracts
{
    public record CreateTaskItemDto(
        string Title,
        string? Description,
        DateOnly DueDate,
        DateTimeOffset? ReminderAt
    );

    public record UpdateTaskItemDto(
        Guid Guid,
        string Title,
        string? Description,
        bool IsDone,
        bool IsActive,
        DateOnly DueDate,
        DateTimeOffset? ReminderAt
    );

    public record ReadTaskItemDto(
        Guid Guid,
        string Title,
        string? Description,
        bool IsDone,
        bool IsActive,
        DateOnly DueDate,
        DateTimeOffset? ReminderAt,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );
}
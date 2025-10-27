using TodoWarrior.Api.Models;

namespace TodoWarrior.Api.Contracts
{
    public static class Mappings
    {
        public static ReadTaskItemDto ToReadDto(this TaskItem taskItem) =>
            new ReadTaskItemDto(
                taskItem.Guid,
                taskItem.Title,
                taskItem.Description,
                taskItem.IsDone,
                taskItem.IsActive,
                taskItem.DueDate,
                taskItem.ReminderAt,
                taskItem.CreatedAt,
                taskItem.UpdatedAt
            );

        public static TaskItem ToModel(this CreateTaskItemDto createDto) =>
            new TaskItem
            {
                Guid = Guid.NewGuid(),
                Title = createDto.Title.Trim(),
                Description = string.IsNullOrEmpty(createDto.Description) ? null : createDto.Description.Trim(),
                IsDone = false,
                DueDate = createDto.DueDate,
                ReminderAt = createDto.ReminderAt,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
        
        public static void UpdateFromDto(this TaskItem taskItem, UpdateTaskItemDto updateDto) 
        {
            taskItem.Title = updateDto.Title.Trim();
            taskItem.Description = string.IsNullOrEmpty(updateDto.Description) ? null : updateDto.Description.Trim();
            taskItem.IsDone = updateDto.IsDone;
            taskItem.DueDate = updateDto.DueDate;
            taskItem.ReminderAt = updateDto.ReminderAt;
            taskItem.Touch();
        }
    }
}
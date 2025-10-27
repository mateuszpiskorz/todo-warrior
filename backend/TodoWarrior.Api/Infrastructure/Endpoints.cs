using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoWarrior.Api.Data;
using TodoWarrior.Api.Models;
using TodoWarrior.Api.Contracts;
using TodoWarrior.Api.Abstractions;
using FluentValidation;


namespace TodoWarrior.Api.Infrastructure
{
    public static class Endpoints
    {
        public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder routes)
        {
            var taskGroup = routes.MapGroup("/api/tasks");

            taskGroup.MapGet("", async (ITaskRepository repo) =>
            {
                var tasks = await repo.GetAllAsync();
                return Results.Ok(tasks);
            });

            taskGroup.MapGet("{id:guid}", async (Guid id, ITaskRepository repo) =>
            {
                var taskItem = await repo.GetByGuidAsync(id);
                return taskItem != null
                    ? Results.Ok(taskItem.ToReadDto())
                    : Results.NotFound();
            });

            taskGroup.MapPost("", async ([FromBody] CreateTaskItemDto dto, IValidator<CreateTaskItemDto> validator, ITaskRepository repo) =>
            {
                var validationResult = await validator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                var taskItem = dto.ToModel();
                await repo.AddAsync(taskItem);
                await repo.SaveChangesAsync();

                return Results.Created($"/api/tasks/{taskItem.Guid}", taskItem);
            });

            taskGroup.MapPut("{id:guid}", async (Guid id, [FromBody] UpdateTaskItemDto dto, IValidator<UpdateTaskItemDto> validator, ITaskRepository repo) =>
            {
                var validationResult = await validator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }
                
                var taskItem = await repo.GetByGuidAsync(id);
                if (taskItem == null)
                {
                    return Results.NotFound();
                }

                taskItem.UpdateFromDto(dto);
                await repo.SaveChangesAsync();

                return Results.Ok(taskItem.ToReadDto());
            });

            taskGroup.MapDelete("{id:guid}", async (Guid id, ITaskRepository repo) =>
            {
                var taskItem = await repo.GetByGuidAsync(id);
                if (taskItem == null)
                {
                    return Results.NotFound();
                }

                await repo.DeleteAsync(taskItem.Guid);
                await repo.SaveChangesAsync();

                return Results.NoContent();
            });

            return routes;
        }
    }
}
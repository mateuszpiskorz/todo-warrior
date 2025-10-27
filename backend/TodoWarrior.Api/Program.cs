using Microsoft.EntityFrameworkCore;
using TodoWarrior.Api.Data;
using TodoWarrior.Api.Infrastructure;
using TodoWarrior.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterServices(builder.Configuration);

var app = builder.Build();

app.EnsureDatabaseMigrationAndSeedData();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");

//Midllware + endpoints
app.UseCors();
app.MapHub<TodoWarrior.Api.Hubs.RemindersHub>("/hubs/reminders");
app.MapTaskEndpoints();
app.MapGet("/", () => "TodoWarrior API is running.");

app.Run();

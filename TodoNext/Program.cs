//#!/usr/bin/env dotnet run!?

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDbContext>(options => options.UseInMemoryDatabase("Todos"));
builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("/api/todos", (TodoDbContext db) => db.Todos.ToListAsync());

app.MapGet("/api/todos/{id}", async (int id, TodoDbContext db) =>
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        return Ok(todo);
    }

    return NotFound();
});

app.MapPost("/api/todos", async (Todo todo, TodoDbContext db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return NoContent();
});

app.MapPost("/api/todos/{id}", async (int id, Todo inputTodo, TodoDbContext db) =>
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        todo.IsComplete = inputTodo.IsComplete;

        await db.SaveChangesAsync();

        return NoContent();
    }

    return NotFound();
});

app.MapDelete("/api/todos/{id}", async (int id, TodoDbContext db) =>
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();

        return NoContent();
    }

    return NotFound();
});

await app.RunAsync();

public class Todo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsComplete { get; set; }
}

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
    {
    }

    public DbSet<Todo> Todos { get; set; }
}

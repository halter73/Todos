using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDbContext>(options => options.UseInMemoryDatabase("Todos"));
builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("/api/todos", (Func<TodoDbContext, Task<List<Todo>>>)GetTodos);
async Task<List<Todo>> GetTodos([FromServices] TodoDbContext db)
{
    return await db.Todos.ToListAsync();
}

app.MapGet("/api/todos/{id}", (Func<int, TodoDbContext, Task<IResult>>)GetTodo);
async Task<IResult> GetTodo([FromRoute] int id, [FromServices] TodoDbContext db)
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null)
    {
        return new NotFoundResult();
    }

    return new JsonResult(todo);
}

app.MapPost("/api/todos", (Func<Todo, TodoDbContext, Task<StatusCodeResult>>)CreateTodo);
async Task<StatusCodeResult> CreateTodo([FromBody] Todo todo, [FromServices] TodoDbContext db)
{
    await db.Todos.AddAsync(todo);
    await db.SaveChangesAsync();

    return new NoContentResult();
}

app.MapPost("/api/todos/{id}", (Func<int, Todo, TodoDbContext, Task<StatusCodeResult>>)UpdateCompleted);
async Task<StatusCodeResult> UpdateCompleted([FromRoute] int id, [FromBody] Todo inputTodo, [FromServices] TodoDbContext db)
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null)
    {
        return new NotFoundResult();
    }

    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();

    return new NoContentResult();
}

app.MapDelete("/api/todos/{id}", (Func<int, TodoDbContext, Task<StatusCodeResult>>)DeleteTodo);
async Task<StatusCodeResult> DeleteTodo([FromRoute] int id, [FromServices] TodoDbContext db)
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null)
    {
        return new NotFoundResult();
    }

    db.Todos.Remove(todo);
    await db.SaveChangesAsync();

    return new NoContentResult();
}

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TodoWithGenericHost
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TodoDbContext>(options => options.UseInMemoryDatabase("Todos"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/api/todos", GetTodos);
                endpoints.MapGet("/api/todos/{id}", GetTodo);
                endpoints.MapPost("/api/todos", CreateTodo);
                endpoints.MapPost("/api/todos/{id}", UpdateCompleted);
                endpoints.MapDelete("/api/todos/{id}", DeleteTodo);
            });
        }

        private static async Task GetTodos(HttpContext context)
        {
            var db = context.RequestServices.GetRequiredService<TodoDbContext>();
            var todos = await db.Todos.ToListAsync();

            await context.Response.WriteAsJsonAsync(todos);
        }

        private static async Task GetTodo(HttpContext context)
        {
            if (!TryGetId(context.Request.RouteValues, out int id))
            {
                context.Response.StatusCode = 400;
                return;
            }

            var db = context.RequestServices.GetRequiredService<TodoDbContext>();
            var todo = await db.Todos.FindAsync(id);
            if (todo is null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            await context.Response.WriteAsJsonAsync(todo);
        }

        private static async Task CreateTodo(HttpContext context)
        {
            var todo = await context.Request.ReadFromJsonAsync<Todo>();

            var db = context.RequestServices.GetRequiredService<TodoDbContext>();
            db.Todos.Add(todo);
            await db.SaveChangesAsync();

            context.Response.StatusCode = 204;
        }

        private static async Task UpdateCompleted(HttpContext context)
        {
            if (!TryGetId(context.Request.RouteValues, out int id))
            {
                context.Response.StatusCode = 400;
                return;
            }

            var db = context.RequestServices.GetRequiredService<TodoDbContext>();
            var todo = await db.Todos.FindAsync(id);

            if (todo is null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            var inputTodo = await context.Request.ReadFromJsonAsync<Todo>();
            todo.IsComplete = inputTodo.IsComplete;

            await db.SaveChangesAsync();

            context.Response.StatusCode = 204;
        }

        private static async Task DeleteTodo(HttpContext context)
        {
            if (!TryGetId(context.Request.RouteValues, out int id))
            {
                context.Response.StatusCode = 400;
                return;
            }

            var db = context.RequestServices.GetRequiredService<TodoDbContext>();
            var todo = await db.Todos.FindAsync(id);
            if (todo is null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            db.Todos.Remove(todo);
            await db.SaveChangesAsync();

            context.Response.StatusCode = 204;
        }

        private static bool TryGetId(RouteValueDictionary routeValues, out int id)
        {   
            if (routeValues.TryGetValue("id", out object value) && value is string unparsedId)
            {
                if (int.TryParse(unparsedId, out id))
                {
                    return true;
                }
            }

            id = 0;
            return false;
        }
    }

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
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace TodoWithControllers.Controllers
{
    [ApiController]
    [Route("/api/todos")]
    public class TodoController : ControllerBase
    {
        private readonly TodoDbContext _db;

        public TodoController(TodoDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        [HttpGet]
        public async Task<ActionResult<List<Todo>>> GetTodos()
        {
            return await _db.Todos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Todo>> GetTodo(int id)
        {
            if (await _db.Todos.FindAsync(id) is Todo todo)
            {
                return todo;
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTodo(Todo todo)
        {
            _db.Todos.Add(todo);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateCompleted(int id, Todo inputTodo)
        {
            if (await _db.Todos.FindAsync(id) is Todo todo)
            {
                todo.IsComplete = inputTodo.IsComplete;

                await _db.SaveChangesAsync();

                return NoContent();
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var todo = await _db.Todos.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            _db.Todos.Remove(todo);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoListAppServer.Data;
using ToDoListAppServer.Dto;
using ToDoListAppServer.Models;

namespace MyWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly ToDoListDbContext _context;

        public TodosController(ToDoListDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoDTO>>> GetTodos(string UserId)
        {
            try
            {
                if (_context.Todos == null)
                {
                    return NotFound();
                }

                var todoItems = await _context.Todos
                    .Where(x => x.UserId == UserId)
                    .Select(x => ItemToDTO(x))
                    .ToListAsync();

                return todoItems;
            }
            catch (Exception ex)
            {
                // Log dell'eccezione (IMPORTANTE)
                Console.WriteLine($"Error in GetTodos: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoDTO>> GetTodo(int id)
        {
            if (_context.Todos == null)
            {
                return NotFound();
            }
            var todoItem = await _context.Todos.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return ItemToDTO(todoItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(int id, TodoDTO todoDTO)
        {
            var todoItem = await _context.Todos.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            todoItem.Title = todoDTO.Title;
            todoItem.Description = todoDTO.Description;
            todoItem.IsCompleted = todoDTO.IsCompleted;

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<TodoDTO>> PostTodoItem(string userId, TodoDTO todoDTO)
        {
            if (_context.Todos == null)
            {
                return Problem("Entity set 'ToDoListDbContext.Todos'  is null.");
            }

            var todoItem = new Todo()
            {
                Description = todoDTO.Description,
                IsCompleted = false,
                Title = todoDTO.Title,
                UserId = userId
            };

            _context.Todos.Add(todoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodo), new { id = todoItem.Id }, todoItem);

        }

        // DELETE: api/Todos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            if (_context.Todos == null)
            {
                return NotFound();
            }
            var todoItem = await _context.Todos.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.Todos.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemExists(int id)
        {
            return (_context.Todos?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        //Crea un metodo che trasforma un todo in un todoDTO
        private static TodoDTO ItemToDTO(Todo todo) =>
            new TodoDTO
            {
                Id = todo.Id,
                Title = todo.Title,
                IsCompleted = todo.IsCompleted,
                Description = todo.Description
            };
    }
}
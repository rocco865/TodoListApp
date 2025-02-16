using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoListAppServer.Data;
using ToDoListAppServer.Models;

namespace ToDoListAppServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly ToDoListDbContext _context;

        public RegistrationController(ToDoListDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                // Crea un nuovo utente
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = model.Password // ATTENZIONE: NON USARE IN PRODUZIONE!
                };

                // Aggiungi l'utente al database
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Restituisci una risposta di successo
                return Ok(new { Message = "Registration successful" });
            }

            // Se il modello non è valido, restituisci un errore
            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user != null)
                {
                    if (user.PasswordHash == model.PasswordHash)
                    {
                        // Autenticazione riuscita
                        return Ok(new { Message = "Login successful", Email = user.Email });
                    }
                    else
                    {
                        return Unauthorized(new { Message = "Invalid credentials" });
                    }

                }
                else
                {
                    //Email non trovata
                    return Unauthorized(new { Message = "Invalid credentials" });
                }
            }

            // Se il modello non è valido, restituisci un errore
            return BadRequest(ModelState);
        }
    }
}
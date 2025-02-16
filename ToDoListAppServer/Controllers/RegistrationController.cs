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
                //Verifica che l'email non sia già usata
                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    return BadRequest("Email già in uso");
                }
                // Crea un nuovo utente
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = model.Password // ***MAI FARE IN PRODUZIONE!***
                };

                // Aggiungi l'utente al database
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Restituisci una risposta di successo
                return Ok("User created successfully");
            }

            // Se il modello non è valido, restituisci un errore
            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
        {
            if (ModelState.IsValid)
            {
                // Trova l'utente nel database
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user == null)
                {
                    return BadRequest("Credenziali non valide");
                }

                // Verifica la password (IN PRODUZIONE, USA L'HASHING!)
                if (user.PasswordHash != model.Password)  // ***MAI FARE IN PRODUZIONE!***
                {
                    return BadRequest("Credenziali non valide");
                }

                // Login effettuato con successo
                return Ok("Login effettuato con successo");
            }

            return BadRequest(ModelState);
        }
    }
}
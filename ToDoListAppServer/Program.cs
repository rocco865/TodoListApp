using Microsoft.EntityFrameworkCore;
using ToDoListAppServer.Data;

var builder = WebApplication.CreateBuilder(args);

//Configura EF Core con PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ToDoListDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configurazione del CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorWasm",  // Definisci un nome per la policy
        policy =>
        {
            policy.WithOrigins("http://localhost:5282") 
                   .AllowAnyMethod() 
                   .AllowAnyHeader(); 
        });
});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowBlazorWasm");

app.UseAuthorization();

app.MapControllers();

app.Run();
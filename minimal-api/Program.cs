
using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.DTOs;
using minimal_api.Infraestrutura.Db;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<DbContexto>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("postgresql");
    options.UseNpgsql(connectionString);
});


var app = builder.Build();
app.MapGet("/", () => "Good morning!");

app.MapPost("/login", (LoginDTO loginDTO) =>
{
  if (loginDTO.Email == "dani@test.com" && loginDTO.Senha == "senha123")
    return Results.Ok("Login successful");
  else
    return Results.Unauthorized();
});

app.Run();



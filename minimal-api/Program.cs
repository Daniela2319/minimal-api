
var builder = WebApplication.CreateBuilder(args);
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

// DTO class for login
public class LoginDTO
{
  public string Email { get; set; } = default!;
  public string Senha { get; set; } = default!;

}
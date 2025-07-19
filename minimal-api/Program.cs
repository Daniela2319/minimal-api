
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;
using minimal_api.Dominio.ModelViews;
using minimal_api.Dominio.Servicos;
using minimal_api.Infraestrutura.Db;
using Scalar.AspNetCore;

#region Builder
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();
builder.Services.AddOpenApi();


builder.Services.AddDbContext<DbContexto>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("postgresql");
    options.UseNpgsql(connectionString);
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion  

#region Administrador
app.MapPost("/administradores/login", ([FromBody]LoginDTO loginDTO, IAdministradorServico administradorServico) =>
{
  if (administradorServico.Login(loginDTO) != null)
    return Results.Ok("Login successful");
  else
    return Results.Unauthorized();
}).WithTags("Administrador");
#endregion

#region Veiculos
ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new ErrosDeValidacao
    {
        Mensagens = new List<string>()
    };

    if (string.IsNullOrEmpty(veiculoDTO.Nome) || string.IsNullOrEmpty(veiculoDTO.Marca) || string.IsNullOrEmpty(veiculoDTO.Ano))
        validacao.Mensagens.Add("Nome, Marca e Ano são obrigatórios.");

    return validacao;
}

// Criar Veículo
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var validacao = validaDTO(veiculoDTO);
    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = int.Parse(veiculoDTO.Ano)

    };
    veiculoServico.Adicionar(veiculo);
    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
}).WithTags("Veículo");

// Obter todos
app.MapGet("/veiculos", ([FromQuery] int? pagina, [FromQuery] string? nome, [FromQuery] string? marca, IVeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.ObterTodos(pagina, nome, marca);
    return Results.Ok(veiculos);
});

// Obter por ID
app.MapGet("/veiculos/{id:int}", (int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.ObterPorId(id);
    if (veiculo == null)
        return Results.NotFound();
    return Results.Ok(veiculo);
}).WithTags("Veículo");

//atualizar
app.MapPut("/veiculos/{id:int}", (int id, [FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var validacao = validaDTO(veiculoDTO);
    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    var veiculo = veiculoServico.ObterPorId(id);
    if (veiculo == null)
        return Results.NotFound();
    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = int.Parse(veiculoDTO.Ano);
    veiculoServico.Atualizar(veiculo);
    return Results.Ok(veiculo);
}).WithTags("Veículo");

//excluir
app.MapDelete("/veiculos/{id:int}", (int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.ObterPorId(id);
    if (veiculo == null)
        return Results.NotFound();
    veiculoServico.Excluir(veiculo);
    return Results.NoContent();
}).WithTags("Veículo");

#endregion

#region App
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Expõe o endpoint OpenAPI
    app.MapScalarApiReference(); // Ativa a interface Scalar
}

app.Run();
#endregion


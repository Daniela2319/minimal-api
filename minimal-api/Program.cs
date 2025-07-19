
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Enus;
using minimal_api.Dominio.Interfaces;
using minimal_api.Dominio.ModelViews;
using minimal_api.Dominio.Servicos;
using minimal_api.Infraestrutura.Db;
using Scalar.AspNetCore;

#region Builder
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration["JWT:SecretKey"] ?? throw new Exception("Chave JWT não configurada.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(option =>
    {
        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();


builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();


builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

builder.Services.AddDbContext<DbContexto>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("postgresql");
    options.UseNpgsql(connectionString);
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
#endregion  

#region Administrador

string GerarTokenJwt(Administrador administrador)
{
    if (string.IsNullOrEmpty(key)) return string.Empty;
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim("Email", administrador.Email),
        new Claim("Perfil", administrador.Perfil),
        new Claim(ClaimTypes.Role, administrador.Perfil),

    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

ErrosDeValidacao validaDTO1(AdministradorDTO administradorDTO)
{
    var validacao = new ErrosDeValidacao
    {
        Mensagens = new List<string>()
    };

    if (string.IsNullOrEmpty(administradorDTO.Email) || string.IsNullOrEmpty(administradorDTO.Senha)) 
        validacao.Mensagens.Add("Nome, Marca e Ano são obrigatórios.");

    return validacao;
}

app.MapPost("/administradores/login", ([FromBody]LoginDTO loginDTO, IAdministradorServico administradorServico) =>
{
    var adm = administradorServico.Login(loginDTO);
    if (adm != null)
    {
        string token = GerarTokenJwt(adm);

        return Results.Ok(new AdmLogado
        {

            Email = adm.Email,
            Perfil = adm.Perfil,
            Token = token
        });
    }
    else
        return Results.Unauthorized();
}).AllowAnonymous().WithTags("Administradores");

//list
app.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
{
    var adms = new List<AdministradorModelView>();
    var administradores = administradorServico.Todos(pagina);
    foreach (var adm in administradores)
    {
        adms.Add(new AdministradorModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Perfil =adm.Perfil
        });
    }
    return Results.Ok(adms);
})
  .RequireAuthorization()
  .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm"})
  .WithTags("Administradores");

//list id
app.MapGet("/administradores/{id:int}", (int id, IAdministradorServico administradorServico) =>
{
    var administrador = administradorServico.BuscarPorId(id);
    if (administrador == null)
        return Results.NotFound();
    return Results.Ok(new AdministradorModelView
    {
        Id = administrador.Id,
        Email = administrador.Email,
        Perfil = administrador.Perfil
    });
})
    .RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
    .WithTags("Administradores");

//post
app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) => {

    var validacao = validaDTO1(administradorDTO);
    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    var adm = new Administrador
    {
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil?.ToString() ?? Perfil.Editor.ToString(),
    };
    
    administradorServico.Incluir(adm);
    return Results.Created($"/administrador/{adm.Id}", new AdministradorModelView
    {
        Id = adm.Id,
        Email = adm.Email,
        Perfil = adm.Perfil
    }); 


})
    .RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
    .WithTags("Administradores");
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
})
    .RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
    .WithTags("Veículos");

// Obter todos
app.MapGet("/veiculos", ([FromQuery] int? pagina, [FromQuery] string? nome, [FromQuery] string? marca, IVeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.ObterTodos(pagina, nome, marca);
    return Results.Ok(veiculos);
}).RequireAuthorization().WithTags("Veículos");

// Obter por ID
app.MapGet("/veiculos/{id:int}", (int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.ObterPorId(id);
    if (veiculo == null)
        return Results.NotFound();
    return Results.Ok(veiculo);
})
    .RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" })
    .WithTags("Veículos");

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
})
    .RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
    .WithTags("Veículos");

//excluir
app.MapDelete("/veiculos/{id:int}", (int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.ObterPorId(id);
    if (veiculo == null)
        return Results.NotFound();
    veiculoServico.Excluir(veiculo);
    return Results.NoContent();
})
    .RequireAuthorization()
    .RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
    .WithTags("Veículos");

#endregion

#region App

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Expõe o endpoint OpenAPI
    app.MapScalarApiReference(); // Ativa a interface Scalar   
}



app.Run();
#endregion


using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.IdentityModel.Tokens;
using MyBancoApi.Transferencia.Application.Commands.EfetuarTransferencia;
using MyBancoApi.Transferencia.Application.Exceptions;
using MyBancoApi.Transferencia.Application.Interfaces;
using MyBancoApi.Transferencia.Domain.Interfaces;
using MyBancoApi.Transferencia.Infrastructure.Clients;
using MyBancoApi.Transferencia.Infrastructure.Database;
using MyBancoApi.Transferencia.Infrastructure.Repositories;
using System.Text;
using System.IdentityModel.Tokens.Jwt; // Para JwtRegisteredClaimNames
using Microsoft.AspNetCore.Http; // Para StatusCodes

var builder = WebApplication.CreateBuilder(args);

// --- Configuração das seções do appsettings ---
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Key"];
// ---------------------------------------------

// 1. Adicionar Serviços (Injeção de Dependência)
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(EfetuarTransferenciaCommand).Assembly));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. Injeção de Dependência da Infraestrutura
// *** MUDANÇA PARA DOCKER ***
// Trocamos de "TransferenciaDB" para um arquivo físico
var connectionString = "Data Source=/db/transferencia.db";
var keepAliveConnection = new SqliteConnection(connectionString);
keepAliveConnection.Open();
builder.Services.AddSingleton<SqliteConnection>(keepAliveConnection);
// *** FIM DA MUDANÇA ***

// Registra os Repositórios Dapper
builder.Services.AddScoped<ITransferenciaRepository, TransferenciaRepository>();
builder.Services.AddScoped<IIdempotenciaRepository, IdempotenciaRepository>();

// 3. Configurar o HttpClient (para chamar a API Conta Corrente)
builder.Services.AddHttpClient("ContaCorrenteApi", client =>
{
    // Pega a URL do appsettings.json
    client.BaseAddress = new Uri(builder.Configuration["ApiClients:ContaCorrenteApiUrl"]);
});
builder.Services.AddScoped<IContaCorrenteApiClient, ContaCorrenteApiClient>();


// 4. Configurar Autenticação JWT (para VALIDAR o token)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});
builder.Services.AddAuthorization();


var app = builder.Build();

// 5. Configurar o Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Inicializa o banco de dados deste microsserviço
    var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetService<SqliteConnection>();
    new DbInitializer(db).Initialize();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // <-- Importante
app.UseAuthorization();  // <-- Importante


// 6. Mapear o Endpoint de Transferência
app.MapPost("/api/transferencia", async (EfetuarTransferenciaCommand command, HttpContext context, IMediator mediator) =>
{
    try
    {
        // Pega o ID da conta logada (do Token)
        var idContaCorrenteLogada = context.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

        // Pega o Token JWT "cru" (para repassar à outra API)
        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(idContaCorrenteLogada) || string.IsNullOrEmpty(token))
        {
            return Results.Unauthorized();
        }

        // Injeta os dados do usuário no Comando
        command.IdContaCorrenteLogada = idContaCorrenteLogada;
        command.Token = token;

        // Envia para o Handler (que fará a orquestração)
        await mediator.Send(command);

        // Requisito: "Retornar HTTP 204 em caso de sucesso"
        return Results.NoContent();
    }
    catch (CustomValidationException ex)
    {
        // Requisito: "Retornar HTTP 400 caso os dados estejam inconsistentes"
        return Results.BadRequest(new { Mensagem = ex.Message, TipoFalha = ex.TipoFalha });
    }
    catch (Exception)
    {
        return Results.StatusCode(StatusCodes.Status501NotImplemented);
    }
})
.WithTags("Transferência")
.WithSummary("Realiza uma transferência entre contas.")
.RequireAuthorization(); // <-- Protege o endpoint


app.Run();
using MediatR;
using Microsoft.Data.Sqlite;
using MyBancoApi.ContaCorrente.Application.Commands.CadastrarConta;
using MyBancoApi.ContaCorrente.Domain.Interfaces;
using MyBancoApi.ContaCorrente.Infrastructure.Database;
using MyBancoApi.ContaCorrente.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MyBancoApi.ContaCorrente.Application.Interfaces;
using MyBancoApi.ContaCorrente.Infrastructure.Security;
using MyBancoApi.ContaCorrente.Application.Commands.EfetuarLogin;
using System.Security.Claims;
using MyBancoApi.ContaCorrente.Application.Commands.Movimentacao;
using MyBancoApi.ContaCorrente.Application.Queries.ConsultarSaldo;
using MyBancoApi.ContaCorrente.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models; // <-- 1. ADICIONE ESTE USING

var builder = WebApplication.CreateBuilder(args);

// --- Configuração do JWT ---
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Key"];

builder.Services.AddSingleton(jwtSettings);
// -------------------------


// 1. Adicionar Serviços (Injeção de Dependência)
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CadastrarContaCommand).Assembly));

builder.Services.AddEndpointsApiExplorer();

// 2. *** AJUSTE AQUI: CONFIGURAR O SWAGGER PARA ENTENDER O JWT ***
builder.Services.AddSwaggerGen(options =>
{
    // Adiciona a definição de segurança "Bearer" (para o botão Authorize)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http, // Usar Http para Bearer
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu_token}"
    });

    // Adiciona o requisito de segurança (o "cadeado" nos endpoints)
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
// --- FIM DO AJUSTE ---


// 3. Injeção de Dependência da Infraestrutura

// *** MUDANÇA PARA DOCKER ***
// Trocamos de "InMemorySample" para um arquivo físico que será salvo no volume do Docker.
var connectionString = "Data Source=/db/contacorrente.db";
var keepAliveConnection = new SqliteConnection(connectionString);
keepAliveConnection.Open(); // Abre a conexão para criar o pool e o arquivo
builder.Services.AddSingleton<SqliteConnection>(keepAliveConnection);
// *** FIM DA MUDANÇA ***

// Registra TODOS os Repositórios
builder.Services.AddScoped<IContaCorrenteRepository, ContaCorrenteRepository>();
// ... (IMovimentoRepository, IIdempotenciaRepository, IJwtTokenGenerator) ...
builder.Services.AddScoped<IMovimentoRepository, MovimentoRepository>();
builder.Services.AddScoped<IIdempotenciaRepository, IdempotenciaRepository>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();


// 4. Configurar Autenticação JWT
builder.Services.AddAuthentication(options =>
// ... (resto do código igual) ...
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
// ... (resto do código igual) ...
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
    // ... (resto do código igual) ...
    app.UseSwagger();
    app.UseSwaggerUI();

    var scope = app.Services.CreateScope();
    // ... (resto do código igual) ...
    var db = scope.ServiceProvider.GetService<SqliteConnection>();
    new DbInitializer(db).Initialize();
}

app.UseHttpsRedirection();

// ATIVAR Autenticação e Autorização
app.UseAuthentication();
app.UseAuthorization();


// 6. Mapear os Endpoints
// ... (Todo o mapeamento de endpoints 'MapPost' e 'MapGet' permanece o mesmo) ...
// ... (Endpoint de Cadastro) ...
app.MapPost("/api/contas", async (CadastrarContaCommand command, IMediator mediator) =>
{
    var response = await mediator.Send(command);
    if (!response.Sucesso)
    {
        return Results.BadRequest(new { response.Mensagem, response.TipoFalha });
    }
    return Results.Ok(new { response.IdContaCorrente, response.NumeroConta });
})
.WithTags("Conta Corrente")
.WithSummary("Cadastra uma nova conta corrente.");


// ... (Endpoint de Login) ...
app.MapPost("/api/contas/login", async (EfetuarLoginCommand command, IMediator mediator) =>
{
    var response = await mediator.Send(command);
    if (!response.Sucesso)
    {
        return Results.Json(new { response.Mensagem, response.TipoFalha }, statusCode: StatusCodes.Status401Unauthorized);
    }
    return Results.Ok(new { response.Token });
})
.WithTags("Conta Corrente")
.WithSummary("Efetua o login e retorna um token JWT.");


// ... (Endpoint de Movimentação) ...
app.MapPost("/api/contas/movimentacao", async (MovimentacaoCommand command, HttpContext context, IMediator mediator) =>
{
    var idContaCorrenteLogada = context.User.Claims.FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
    if (string.IsNullOrEmpty(idContaCorrenteLogada))
    {
        return Results.Unauthorized();
    }

    command.IdContaCorrenteLogada = idContaCorrenteLogada;

    var response = await mediator.Send(command);

    if (!response.Sucesso)
    {
        return Results.BadRequest(new { response.Mensagem, response.TipoFalha });
    }

    return Results.NoContent();
})
.WithTags("Conta Corrente")
.WithSummary("Realiza uma movimentação (Crédito ou Débito).")
.RequireAuthorization();


// ... (Endpoint de Saldo) ...
app.MapGet("/api/contas/saldo", async (HttpContext context, IMediator mediator) =>
{
    try
    {
        var idContaCorrenteLogada = context.User.Claims.FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(idContaCorrenteLogada))
        {
            return Results.Unauthorized();
        }

        var query = new ConsultarSaldoQuery { IdContaCorrenteLogada = idContaCorrenteLogada };
        var response = await mediator.Send(query);

        return Results.Ok(response);
    }
    catch (CustomValidationException ex)
    {
        return Results.BadRequest(new { Mensagem = ex.Message, TipoFalha = ex.TipoFalha });
    }
    catch (Exception)
    {
        return Results.StatusCode(500);
    }
})
.WithTags("Conta Corrente")
.WithSummary("Consulta o saldo da conta corrente logada.")
.RequireAuthorization();


app.Run();
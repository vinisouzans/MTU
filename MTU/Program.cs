using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MTU.Data;
using MTU.Services;
using MTU.Services.Interfaces;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConnection>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var factory = new ConnectionFactory
    {
        HostName = configuration["RabbitMQ:HostName"],
        UserName = configuration["RabbitMQ:UserName"],
        Password = configuration["RabbitMQ:Password"],
        DispatchConsumersAsync = true,
        AutomaticRecoveryEnabled = true,
        NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
    };

    var policy = Policy.Handle<BrokerUnreachableException>()
                      .Or<SocketException>()
                      .WaitAndRetry(6, retryAttempt =>
                          TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    return policy.Execute(() => factory.CreateConnection());
});

builder.Services.AddSingleton<IConnectionFactory>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return new ConnectionFactory
    {
        HostName = configuration["RabbitMQ:HostName"],
        UserName = configuration["RabbitMQ:UserName"],
        Password = configuration["RabbitMQ:Password"],
        DispatchConsumersAsync = true,
        AutomaticRecoveryEnabled = true
    };
});

// Adicione serviços de autenticação JWT
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"erro\":\"Você precisa estar autenticado para acessar este recurso.\"}");
        },
        OnForbidden = context =>
        {
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"erro\":\"Você não tem permissão para acessar este recurso.\"}");
        }
    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "MTU API", Version = "v1" });

    // Adiciona suporte a JWT Bearer
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Cole seu token JWT"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddScoped<IEntregadorService, EntregadorService>();
builder.Services.AddScoped<IMotoService, MotoService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ILocacaoService, LocacaoService>();
builder.Services.AddHostedService<MotoCadastradaConsumer>();
builder.Services.AddScoped<MTU.Services.Interfaces.IMotoPublisher, MTU.Services.RabbitMqPublisher>();
builder.Services.AddScoped<IEntregaService, EntregaService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
        Console.WriteLine("Migrações aplicadas com sucesso!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao aplicar migrações: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.Map("/error", (HttpContext context) =>
{
    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    return Results.Problem(
        title: "Erro inesperado",
        detail: exception?.Message,
        statusCode: exception is ArgumentException ? 400 : 500
    );
});

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
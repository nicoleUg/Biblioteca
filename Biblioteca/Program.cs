using System.Text;
using Biblioteca.Core.DTOs;
using Biblioteca.Core.Interfaces;
using Biblioteca.Core.Services;
using Biblioteca.Infrastructure.Data;
using Biblioteca.Infrastructure.Mappings;
using Biblioteca.Infrastructure.Repositories;
using Biblioteca.Infrastructure.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1) BASE DE DATOS (MySQL)

// OJO: el nombre debe coincidir con tu user-secret: "ConnectionStrings:ConnectionMySql"
var connectionString = builder.Configuration.GetConnectionString("MySql");
builder.Services.AddDbContext<BibliotecaContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Dapper
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<IDapperContext, DapperContext>();

// 2) REPOSITORIOS / UNIT OF WORK


builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<ILibroRepository, LibroRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IPrestamoRepository, PrestamoRepository>();
builder.Services.AddScoped<IMultaRepository, MultaRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// 3) SERVICIOS (LÓGICA DE NEGOCIO)

builder.Services.AddScoped<IPrestamoService, PrestamoService>();
builder.Services.AddScoped<IMultaService, MultaService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// 4) AUTOMAPPER + VALIDATION


builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddValidatorsFromAssemblyContaining<UsuarioDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PrestamoDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<MultaDtoValidator>();


// 5) AUTHENTICATION - JWT


var authSection = builder.Configuration.GetSection("Authentication");
var secretKey = authSection["SecretKey"]; //user-secrets

if (string.IsNullOrWhiteSpace(secretKey))
{
    throw new Exception("Falta configurar Authentication:SecretKey en user-secrets.");
}

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; //  poner true en prduccion
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = authSection["Issuer"],
        ValidAudience = authSection["Audience"],
        IssuerSigningKey = key
    };
});


// 6) CONTROLLERS + JSON


builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling =
            Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });


// 7) VERSIONAMIENTO DE API


builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);

    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("x-api-version"),
        new QueryStringApiVersionReader("api-version")
    );
});

// Explorar versiones para Swagger
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";             // v1, v1.0, etc.
    options.SubstituteApiVersionInUrl = true;      // reemplaza {version:apiVersion} en las rutas
});



// 8) SWAGGER (con XML + JWT)


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Biblioteca API",
        Version = "v1",
        Description = "Sistema de Préstamo y Devolución de Libros en una Biblioteca",
        Contact = new()
        {
            Name = "Sofia Nicole Ugarte Salazar",
            Email = "sofia.ugarte@ucb.edu.bo"
        }
    });

    // XML comments (para summary en Swagger)
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // Configuración para JWT en Swagger (botón Authorize)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT con el formato: Bearer {token}"
    });

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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();


// 9) MIDDLEWARE PIPELINE


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Biblioteca API v1");
        c.DocumentTitle = "Documentación API - Biblioteca";
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();  // 🔐 primero autenticación
app.UseAuthorization();   // luego autorización

app.MapControllers();

app.Run();

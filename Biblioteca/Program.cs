using Biblioteca.Core.DTOs;
using Biblioteca.Core.Interfaces;
using Biblioteca.Core.Services;
using Biblioteca.Infrastructure.Data;
using Biblioteca.Infrastructure.Mappings;
using Biblioteca.Infrastructure.Repositories;
using Biblioteca.Infrastructure.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// CONFIGURACIÓN DE BASE DE DATOS


// Entity Framework Core con MySQL
var connectionString = builder.Configuration.GetConnectionString("MySql");
builder.Services.AddDbContext<BibliotecaContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Dapper 
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<IDapperContext, DapperContext>();

// REGISTRO DE REPOSITORIOS

builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<ILibroRepository, LibroRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IPrestamoRepository, PrestamoRepository>();
builder.Services.AddScoped<IMultaRepository, MultaRepository>();

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// REGISTRO DE SERVICIOS

builder.Services.AddScoped<IPrestamoService, PrestamoService>();
builder.Services.AddScoped<IMultaService, MultaService>();

// AUTOMAPPER Y VALIDADORES

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<UsuarioDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PrestamoDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<MultaDtoValidator>();


// CONFIGURACIÓN DE CONTROLADORES


builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling =
            Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });
//Versionamiento 

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
// SWAGGER


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new ()
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
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});



var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Biblioteca API v1");
    });
}

app.UseHttpsRedirection();
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
app.UseAuthorization();
app.MapControllers();

app.Run();
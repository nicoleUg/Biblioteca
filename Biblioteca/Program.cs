using Biblioteca.Core.Interfaces;
using Biblioteca.Core.Services;
using Biblioteca.Infrastructure.Data;
using Biblioteca.Infrastructure.Mappings;
using Biblioteca.Infrastructure.Repositories;
using Biblioteca.Infrastructure.Validators;
using FluentValidation;
using Biblioteca.Core.DTOs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("MySql");
builder.Services.AddDbContext<BibliotecaContext>(opt =>
    opt.UseMySql(cs, ServerVersion.AutoDetect(cs)));

builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IPrestamoService, PrestamoService>();
builder.Services.AddScoped<IMultaService, MultaService>();


builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddValidatorsFromAssemblyContaining<PrestamoCreateDtoValidator>();

builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    o.SerializerSettings.ReferenceLoopHandling =
        Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

var app = builder.Build();
app.MapControllers();
app.Run();

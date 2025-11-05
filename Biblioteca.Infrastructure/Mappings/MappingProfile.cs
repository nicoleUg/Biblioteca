using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Biblioteca.Core.DTOs;
using Biblioteca.Core.Entities;

namespace Biblioteca.Infrastructure.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Libro, LibroDto>().ReverseMap();
        CreateMap<Usuario, UsuarioDto>().ReverseMap();

        CreateMap<Prestamo, PrestamoDto>()
            .ForMember(d => d.FechaPrestamo, m => m.MapFrom(s => s.FechaPrestamo.ToString("dd-MM-yyyy")))
            .ForMember(d => d.FechaLimite, m => m.MapFrom(s => s.FechaLimite.ToString("dd-MM-yyyy")))
            .ForMember(d => d.FechaDevolucion, m => m.MapFrom(s => s.FechaDevolucion.HasValue
                                                                ? s.FechaDevolucion!.Value.ToString("dd-MM-yyyy")
                                                                : null));
        CreateMap<PrestamoDto, Prestamo>()  // usado en PUT si alguna vez lo quisieras
            .ForMember(d => d.FechaPrestamo, m => m.Ignore())
            .ForMember(d => d.FechaLimite, m => m.Ignore())
            .ForMember(d => d.FechaDevolucion, m => m.Ignore());

        CreateMap<Multa, MultaDto>().ReverseMap();
    }
}
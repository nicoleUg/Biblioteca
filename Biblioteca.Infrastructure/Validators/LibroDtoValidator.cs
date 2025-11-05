using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biblioteca.Core.DTOs;

namespace Biblioteca.Infrastructure.Validators;

public class LibroDtoValidator: AbstractValidator<LibroDto>
{
    public LibroDtoValidator()
    {
        RuleFor(x => x.Titulo).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Autor).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Categoria).NotEmpty().MaximumLength(80);
        RuleFor(x => x.TotalEjemplares).GreaterThanOrEqualTo(0);
        RuleFor(x => x.EjemplaresDisponibles)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(x => x.TotalEjemplares);
        RuleFor(x => x.CondicionGeneral).Must(v => new[] { "New", "Good", "Fair", "Poor" }.Contains(v));
    }
}

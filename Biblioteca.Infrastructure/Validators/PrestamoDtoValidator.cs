using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biblioteca.Core.DTOs;
using FluentValidation;
using System.Globalization;

namespace Biblioteca.Infrastructure.Validators;

public class PrestamoDtoValidator : AbstractValidator<PrestamoDto>
{
    public PrestamoDtoValidator()
    {
        RuleFor(x => x.UsuarioId).GreaterThan(0);
        RuleFor(x => x.LibroId).GreaterThan(0);
        RuleFor(x => x.FechaPrestamo)
            .NotEmpty()
            .Must(f => DateTime.TryParseExact(f, "dd-MM-yyyy", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out _))
            .WithMessage("La fecha debe tener el formato dd-MM-yyyy");
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biblioteca.Core.DTOs;
using FluentValidation;

namespace Biblioteca.Infrastructure.Validators;

public class MultaDtoValidator : AbstractValidator<MultaDto>
{
    public MultaDtoValidator()
    {
        RuleFor(x => x.PrestamoId).GreaterThan(0);
        RuleFor(x => x.UsuarioId).GreaterThan(0);
        RuleFor(x => x.Motivo).NotEmpty().MaximumLength(80);
        RuleFor(x => x.MontoBs).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Estado)
            .Must(e => e == "Pending" || e == "Paid")
            .WithMessage("Estado permitido: Pending | Paid");
    }
}
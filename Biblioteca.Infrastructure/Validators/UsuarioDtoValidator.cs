using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biblioteca.Core.DTOs;
using FluentValidation;

namespace Biblioteca.Infrastructure.Validators
{
    public class UsuarioDtoValidator : AbstractValidator<UsuarioDto>
    {
        public UsuarioDtoValidator()
        {
            RuleFor(x => x.Nombre).NotEmpty().MaximumLength(120);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Rol)
                .Must(r => r == "estudiante" || r == "staff")
                .WithMessage("Rol permitido: estudiante | staff");
        }
    }
}

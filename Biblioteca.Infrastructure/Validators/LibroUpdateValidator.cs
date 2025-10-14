using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biblioteca.Core.Entities;
using FluentValidation;
namespace Biblioteca.Infrastructure.Validators;
public class LibroUpdateValidator : AbstractValidator<Libro>
{
    public LibroUpdateValidator()
    {
        RuleFor(x => x.CondicionGeneral).Must(v => new[] { "New", "Good", "Fair", "Poor" }.Contains(v))
            .WithMessage("CondicionGeneral debe ser: New | Good | Fair | Poor");
    }
}

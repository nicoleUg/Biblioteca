using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biblioteca.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Biblioteca.Infrastructure.Data.Configurations;

public class LibroConfiguration : IEntityTypeConfiguration<Libro>
{
    public void Configure(EntityTypeBuilder<Libro> b)
    {
        b.ToTable("Libro");
        b.HasKey(x => x.Id);
        b.Property(x => x.Titulo).IsRequired().HasMaxLength(200);
        b.Property(x => x.Autor).IsRequired().HasMaxLength(120);
        b.Property(x => x.Categoria).IsRequired().HasMaxLength(80);
        b.Property(x => x.CostoReposicionBs).HasColumnType("decimal(12,2)");
        b.Property(x => x.TotalEjemplares).IsRequired();
        b.Property(x => x.EjemplaresDisponibles).IsRequired();
        b.Property(x => x.Habilitado).HasDefaultValue(true);
        b.Property(x => x.CondicionGeneral).IsRequired().HasMaxLength(10);
        b.HasCheckConstraint("CK_Libro_Condicion", "CondicionGeneral IN ('New','Good','Fair','Poor')");
    }
}
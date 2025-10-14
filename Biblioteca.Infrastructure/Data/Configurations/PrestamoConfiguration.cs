using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Biblioteca.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Biblioteca.Infrastructure.Data.Configurations;

public class PrestamoConfiguration : IEntityTypeConfiguration<Prestamo>
{
    public void Configure(EntityTypeBuilder<Prestamo> b)
    {
        b.ToTable("Prestamo");
        b.HasKey(x => x.Id);
        b.Property(x => x.Estado).HasMaxLength(30);

        b.HasOne(x => x.Usuario)
         .WithMany(u => u.Prestamos)
         .HasForeignKey(x => x.UsuarioId);

        b.HasOne(x => x.Libro)
         .WithMany(l => l.Prestamos)
         .HasForeignKey(x => x.LibroId);
    }
}
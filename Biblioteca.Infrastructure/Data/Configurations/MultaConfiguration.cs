using Biblioteca.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Biblioteca.Infrastructure.Data.Configurations;

public class MultaConfiguration : IEntityTypeConfiguration<Multa>
{
    public void Configure(EntityTypeBuilder<Multa> b)
    {
        b.ToTable("Multa");
        b.HasKey(x => x.Id);
        b.Property(x => x.Motivo).IsRequired().HasMaxLength(200);
        b.Property(x => x.Estado).HasMaxLength(20);
        b.Property(x => x.MontoBs).HasColumnType("decimal(12,2)");

        b.HasOne(x => x.Prestamo).WithMany(p => p.Multas).HasForeignKey(x => x.PrestamoId);
        b.HasOne(x => x.Usuario).WithMany(u => u.Multas).HasForeignKey(x => x.UsuarioId);
    }
}
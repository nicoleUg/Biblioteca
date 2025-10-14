

using Biblioteca.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Biblioteca.Infrastructure.DesignTime
{
    // Esta factory solo se usa en tiempo de diseño (Add-Migration / Update-Database)
    public class BibliotecaContextFactory : IDesignTimeDbContextFactory<BibliotecaContext>
    {
        public BibliotecaContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BibliotecaContext>();

            // ⚠️ Usa la misma cadena de conexión que tienes en appsettings.json
            // Cambia la contraseña/puerto según tu MySQL
            var cs = "Server=localhost;Port=3306;Database=DbBiblioteca;Uid=root;Pwd=8825358Nsu;";
            optionsBuilder.UseMySql(cs, ServerVersion.AutoDetect(cs));

            return new BibliotecaContext(optionsBuilder.Options);
        }
    }
}

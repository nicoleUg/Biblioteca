using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biblioteca.Core.Entities;
using Biblioteca.Core.Interfaces;

namespace Biblioteca.Core.Services;

public class PrestamoService : IPrestamoService
{
    private readonly IBaseRepository<Prestamo> _prestamos;
    private readonly IBaseRepository<Usuario> _usuarios;
    private readonly IBaseRepository<Libro> _libros;
    private readonly IBaseRepository<Multa> _multas;

    public PrestamoService(IBaseRepository<Prestamo> p, IBaseRepository<Usuario> u,
        IBaseRepository<Libro> l, IBaseRepository<Multa> m)
    { _prestamos = p; _usuarios = u; _libros = l; _multas = m; }

    public async Task<Prestamo> CrearPrestamoAsync(int usuarioId, int libroId, DateTime fechaPrestamo)
    {
        var usuario = await _usuarios.GetById(usuarioId) ?? throw new Exception("El usuario no existe");
        if (!usuario.Activo) throw new Exception("Usuario inactivo");

        var libro = await _libros.GetById(libroId) ?? throw new Exception("El libro no existe");
        if (!libro.Habilitado) throw new Exception("El libro no está habilitado para préstamo");
        if (libro.EjemplaresDisponibles <= 0) throw new Exception("No hay ejemplares disponibles");
        if (libro.CondicionGeneral == "Poor") throw new Exception("Ejemplar no apto para préstamo");

        var maxPorRol = usuario.Rol == "staff" ? 5 : 3;
        var activos = (await _prestamos.GetAll()).Count(x => x.UsuarioId == usuarioId && x.Estado == "Prestado");
        if (activos >= maxPorRol) throw new Exception($"Límite de préstamos activos alcanzado ({maxPorRol})");

        var prestamo = new Prestamo
        {
            UsuarioId = usuarioId,
            LibroId = libroId,
            FechaPrestamo = fechaPrestamo,
            FechaLimite = fechaPrestamo.AddDays(7),
            Estado = "Prestado"
        };

        libro.EjemplaresDisponibles -= 1;

        await _prestamos.Add(prestamo);
        await _libros.Update(libro);
        return prestamo;
    }

    public async Task<Prestamo?> DevolverPrestamoAsync(int prestamoId, DateTime fechaDevolucion)
    {
        var prestamo = await _prestamos.GetById(prestamoId);
        if (prestamo is null) return null;
        if (prestamo.Estado != "Prestado") throw new Exception("El préstamo no está activo");

        prestamo.FechaDevolucion = fechaDevolucion;
        prestamo.Estado = "Devuelto";

        var libro = await _libros.GetById(prestamo.LibroId)!;
        libro.EjemplaresDisponibles += 1;

        if (fechaDevolucion.Date > prestamo.FechaLimite.Date)
        {
            var dias = (fechaDevolucion.Date - prestamo.FechaLimite.Date).Days;
            var multa = new Multa
            {
                PrestamoId = prestamo.Id,
                UsuarioId = prestamo.UsuarioId,
                Motivo = "Retraso",
                Detalle = $"Atraso de {dias} día(s)",
                MontoBs = dias * 1m,
                Estado = "Pending"
            };
            await _multas.Add(multa);
        }

        // guardamos siempre
        await _prestamos.Update(prestamo);
        await _libros.Update(libro);
        return prestamo;
    }

}
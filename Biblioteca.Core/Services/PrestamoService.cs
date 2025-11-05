using Biblioteca.Core.Entities;
using Biblioteca.Core.Exceptions;
using Biblioteca.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Core.Services
{
    public class PrestamoService : IPrestamoService
    {
        private readonly IUnitOfWork _uow;

        public PrestamoService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        /// <summary>
        /// Crea un préstamo validando disponibilidad y que el usuario no tenga otro préstamo activo.
        /// </summary>
        public async Task<Prestamo> CrearPrestamoAsync(int usuarioId, int libroId, DateTime fechaPrestamo)
        {
            // 1) Validar existencia del usuario
            var usuario = await _uow.Usuarios.GetById(usuarioId);
            if (usuario is null)
                throw new BusinessException("Usuario no existe", 404);
            if (!usuario.Activo)
                throw new BusinessException("Usuario inactivo", 400);

            // 2) Validar existencia del libro
            var libro = await _uow.Libros.GetById(libroId);
            if (libro is null)
                throw new BusinessException("Libro no existe", 404);
            if (!libro.Habilitado)
                throw new BusinessException("Libro no habilitado", 400);

            // 3) Validar préstamos activos del usuario (usando repositorio extendido)
            var prestamosActivos = await _uow.PrestamosEx.GetAllDapperAsync(usuarioId, null, "Prestado");
            var tieneActivo = prestamosActivos.Any();

            if (tieneActivo)
                throw new BusinessException("El usuario ya tiene un préstamo activo", 400);

            // 4) Validar stock del libro
            if (libro.EjemplaresDisponibles <= 0)
                throw new BusinessException("No hay ejemplares disponibles", 400);

            // 5) Crear préstamo
            var prestamo = new Prestamo
            {
                UsuarioId = usuarioId,
                LibroId = libroId,
                Estado = "Prestado",
                FechaPrestamo = fechaPrestamo,
                FechaLimite = fechaPrestamo.AddDays(7),
                FechaDevolucion = null
            };

            // 6) Persistir en transacción
            await _uow.BeginTransactionAsync();
            try
            {
                // Descontar stock
                libro.EjemplaresDisponibles -= 1;
                _uow.Libros.Update(libro);

                await _uow.Prestamos.Add(prestamo);
                await _uow.CommitAsync();
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }

            return prestamo;
        }

        /// <summary>
        /// Registra la devolución; cambia estado, fecha y devuelve stock.
        /// </summary>
        public async Task<Prestamo?> DevolverPrestamoAsync(int id, DateTime fechaDevolucion)
        {
            var prestamo = await _uow.Prestamos.GetById(id);
            if (prestamo is null) return null;

            if (prestamo.Estado == "Devuelto")
                throw new BusinessException("El préstamo ya fue devuelto", 400);

            // Recuperar libro para devolver stock
            var libro = await _uow.Libros.GetById(prestamo.LibroId);
            if (libro is null)
                throw new BusinessException("Libro del préstamo no existe", 404);

            prestamo.Estado = "Devuelto";
            prestamo.FechaDevolucion = fechaDevolucion;

            await _uow.BeginTransactionAsync();
            try
            {
                // Devolver stock
                libro.EjemplaresDisponibles += 1;
                _uow.Libros.Update(libro);
                _uow.Prestamos.Update(prestamo);

                await _uow.CommitAsync();
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }

            return prestamo;
        }
    }
}
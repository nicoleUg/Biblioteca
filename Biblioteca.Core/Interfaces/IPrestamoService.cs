using Biblioteca.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biblioteca.Core.Entities;


namespace Biblioteca.Core.Interfaces
{
    public  interface IPrestamoService
    {
        Task<Prestamo> CrearPrestamoAsync(int usuarioId, int libroId, DateTime fechaPrestamo);
        Task<Prestamo?> DevolverPrestamoAsync(int id, DateTime fechaDevolucion);

    }

}

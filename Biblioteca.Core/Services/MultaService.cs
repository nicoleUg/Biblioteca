using Biblioteca.Core.Entities;
using Biblioteca.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.Services;
public class MultaService : IMultaService
{
    private readonly IBaseRepository<Multa> _multas;
    private readonly IBaseRepository<Prestamo> _prestamos;

    public MultaService(IBaseRepository<Multa> multas, IBaseRepository<Prestamo> prestamos)
    { _multas = multas; _prestamos = prestamos; }

    public async Task<bool> PagarMultaAsync(int multaId)
    {
        var multa = await _multas.GetById(multaId);
        if (multa is null) return false;

        if (multa.Estado != "Paid")
        {
            multa.Estado = "Paid";
            await _multas.Update(multa);
        }

        await CerrarPrestamoSiCorresponde(multa.PrestamoId);
        return true;
    }

    public async Task<bool> CancelarMultaAsync(int multaId, string? motivo = null)
    {
        var multa = await _multas.GetById(multaId);
        if (multa is null) return false;

        if (multa.Estado != "Canceled")
        {
            multa.Estado = "Canceled";
            multa.Detalle = string.IsNullOrWhiteSpace(motivo) ? multa.Detalle : $"{multa.Detalle} (Cancelada: {motivo})";
            await _multas.Update(multa);
        }

        await CerrarPrestamoSiCorresponde(multa.PrestamoId);
        return true;
    }

    private async Task CerrarPrestamoSiCorresponde(int prestamoId)
    {
        var prestamo = await _prestamos.GetById(prestamoId);
        if (prestamo is null) return;

        // Si todas las multas están pagadas o canceladas → cerramos el préstamo si ya fue devuelto
        var todas = await _multas.GetAll();
        var pendientes = todas.Where(m => m.PrestamoId == prestamoId && m.Estado == "Pending");
        if (!pendientes.Any() && prestamo.Estado == "Devuelto")
        {
            prestamo.Estado = "Cerrado";
            await _prestamos.Update(prestamo);
        }
    }
}

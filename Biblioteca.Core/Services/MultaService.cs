using Biblioteca.Core.Entities;
using Biblioteca.Core.Interfaces;

namespace Biblioteca.Core.Services
{
    public class MultaService : IMultaService
    {
        private readonly IUnitOfWork _uow;

        public MultaService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<bool> PagarMultaAsync(int multaId)
        {
            await _uow.BeginTransactionAsync();
            try
            {
                var multa = await _uow.Multas.GetById(multaId);
                if (multa is null) return false;

                if (multa.Estado != "Paid")
                {
                    multa.Estado = "Paid";
                    _uow.Multas.Update(multa);
                }

                await CerrarPrestamoSiCorresponde(multa.PrestamoId);
                await _uow.CommitAsync();
                return true;
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> CancelarMultaAsync(int multaId, string? motivo = null)
        {
            await _uow.BeginTransactionAsync();
            try
            {
                var multa = await _uow.Multas.GetById(multaId);
                if (multa is null) return false;

                if (multa.Estado != "Canceled")
                {
                    multa.Estado = "Canceled";
                    multa.Detalle = string.IsNullOrWhiteSpace(motivo)
                        ? multa.Detalle
                        : $"{multa.Detalle} (Cancelada: {motivo})";
                    _uow.Multas.Update(multa);
                }

                await CerrarPrestamoSiCorresponde(multa.PrestamoId);
                await _uow.CommitAsync();
                return true;
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        private async Task CerrarPrestamoSiCorresponde(int prestamoId)
        {
            var prestamo = await _uow.Prestamos.GetById(prestamoId);
            if (prestamo is null) return;

            var tienePendientes = await _uow.MultasEx.TienePendientesPorPrestamoAsync(prestamoId);

            if (!tienePendientes && prestamo.Estado == "Devuelto")
            {
                prestamo.Estado = "Cerrado";
                _uow.Prestamos.Update(prestamo);
            }
        }

    }
}
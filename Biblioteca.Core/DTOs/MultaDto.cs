using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.DTOs
{
    /// <summary>
    /// Representa una Multa (DTO) en el sistema
    /// </summary>
    /// <remarks>
    /// Este objeto se utiliza para transferir la información de las sanciones monetarias
    /// aplicadas a los usuarios por retrasos o daños en los préstamos.
    /// </remarks>
    public class MultaDto
    {
        /// <summary>
        /// Identificador único de la multa
        /// </summary>
        /// <remarks>
        /// Este campo suele ser ignorado en las operaciones de creación (POST).
        /// </remarks>
        /// <example>1</example>
        public int Id { get; set; }                 // Ignorado en POST

        /// <summary>
        /// Identificador del préstamo asociado a la multa
        /// </summary>
        /// <example>10</example>
        public int PrestamoId { get; set; }

        /// <summary>
        /// Identificador del usuario sancionado
        /// </summary>
        /// <example>5</example>
        public int UsuarioId { get; set; }

        /// <summary>
        /// Motivo principal de la sanción
        /// </summary>
        /// <example>Devolución tardía</example>
        public string Motivo { get; set; } = "Retraso";

        /// <summary>
        /// Detalles adicionales o descripción específica de la sanción
        /// </summary>
        /// <example>El libro se entregó con 2 días de retraso</example>
        public string Detalle { get; set; } = "";

        /// <summary>
        /// Monto de la multa expresado en Bolivianos
        /// </summary>
        /// <example>15.50</example>
        public decimal MontoBs { get; set; }

        /// <summary>
        /// Estado actual de la multa
        /// </summary>
        /// <remarks>
        /// Valores posibles: Pending (Pendiente), Paid (Pagado), Canceled (Cancelado).
        /// </remarks>
        /// <example>Pending</example>
        public string Estado { get; set; } = "Pending";  // Pending | Paid
    }
}
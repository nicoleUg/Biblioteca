using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.Entities;

/// <summary>
/// Representa una Multa o Sanción económica en el sistema
/// </summary>
/// <remarks>
/// Esta entidad almacena los detalles de las penalizaciones aplicadas a los usuarios
/// debido a infracciones como retrasos en la devolución o daños a los libros.
/// </remarks>
public class Multa : BaseEntity
{
    /// <summary>
    /// Identificador del préstamo asociado que originó la multa
    /// </summary>
    /// <example>10</example>
    public int PrestamoId { get; set; }

    /// <summary>
    /// Referencia de navegación al préstamo asociado
    /// </summary>
    public Prestamo? Prestamo { get; set; }

    /// <summary>
    /// Identificador del usuario que recibió la sanción
    /// </summary>
    /// <example>5</example>
    public int UsuarioId { get; set; }

    /// <summary>
    /// Referencia de navegación al usuario sancionado
    /// </summary>
    public Usuario? Usuario { get; set; }

    /// <summary>
    /// Causa principal de la multa
    /// </summary>
    /// <example>Retraso en devolución</example>
    public string Motivo { get; set; } = null!;

    /// <summary>
    /// Detalles adicionales o descripción específica de la infracción
    /// </summary>
    /// <example>El libro se devolvió con 3 días de demora</example>
    public string? Detalle { get; set; }

    /// <summary>
    /// Monto total de la multa expresado en Bolivianos
    /// </summary>
    /// <example>15.00</example>
    public decimal MontoBs { get; set; }

    /// <summary>
    /// Estado actual de la multa
    /// </summary>
    /// <remarks>
    /// Valores permitidos: "Pending" (Pendiente), "Paid" (Pagado), "Canceled" (Cancelado).
    /// </remarks>
    /// <example>Pending</example>
    public string Estado { get; set; } = "Pending"; // Pending, Paid, canceled
}
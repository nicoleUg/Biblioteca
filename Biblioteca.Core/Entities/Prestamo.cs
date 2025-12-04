using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.Entities;

/// <summary>
/// Representa un Préstamo en el sistema
/// </summary>
/// <remarks>
/// Esta entidad transaccional registra la asignación temporal de un libro a un usuario,
/// controlando las fechas de vencimiento y el estado de la devolución.
/// </remarks>
public class Prestamo : BaseEntity
{
    /// <summary>
    /// Identificador del usuario que solicita el préstamo
    /// </summary>
    /// <example>1</example>
    public int UsuarioId { get; set; }

    /// <summary>
    /// Referencia de navegación al usuario solicitante
    /// </summary>
    public Usuario? Usuario { get; set; }

    /// <summary>
    /// Identificador del libro entregado
    /// </summary>
    /// <example>5</example>
    public int LibroId { get; set; }

    /// <summary>
    /// Referencia de navegación al libro prestado
    /// </summary>
    public Libro? Libro { get; set; }

    /// <summary>
    /// Fecha y hora en que se registró el préstamo
    /// </summary>
    /// <example>2025-10-25T10:00:00</example>
    public DateTime FechaPrestamo { get; set; }

    /// <summary>
    /// Fecha máxima para la devolución del libro sin penalización
    /// </summary>
    /// <remarks>
    /// Calculada en base al rol del usuario (ej. 3 días para estudiantes, 5 para staff).
    /// </remarks>
    /// <example>2025-11-01T10:00:00</example>
    public DateTime FechaLimite { get; set; }

    /// <summary>
    /// Fecha real en la que se devolvió el libro
    /// </summary>
    /// <remarks>
    /// Es nula mientras el libro esté en posesión del usuario.
    /// </remarks>
    /// <example>2025-10-30T15:30:00</example>
    public DateTime? FechaDevolucion { get; set; }

    /// <summary>
    /// Estado actual del ciclo de vida del préstamo
    /// </summary>
    /// <remarks>
    /// Valores permitidos: "Prestado" (Activo), "Devuelto" (Libro retornado), "Cerrado" (Sin multas pendientes).
    /// </remarks>
    /// <example>Prestado</example>
    public string Estado { get; set; } = "Prestado"; // Prestado, Devuelto, cerrado

    /// <summary>
    /// Colección de multas generadas asociadas a este préstamo
    /// </summary>
    public ICollection<Multa> Multas { get; set; } = new List<Multa>();
}
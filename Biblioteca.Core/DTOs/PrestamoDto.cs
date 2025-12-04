using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.DTOs;

/// <summary>
/// Representa un Préstamo (DTO) en el sistema
/// </summary>
/// <remarks>
/// Este objeto se utiliza para transferir la información completa de un préstamo,
/// incluyendo fechas clave y estado actual.
/// </remarks>
public class PrestamoDto
{
    /// <summary>
    /// Identificador único del préstamo
    /// </summary>
    /// <remarks>
    /// Este campo es autogenerado y suele ser ignorado en las operaciones de creación (POST).
    /// </remarks>
    /// <example>1</example>
    public int Id { get; set; }                      // Ignorado en POST

    /// <summary>
    /// Identificador del usuario que tiene el libro
    /// </summary>
    /// <example>1</example>
    public int UsuarioId { get; set; }

    /// <summary>
    /// Identificador del libro prestado
    /// </summary>
    /// <example>5</example>
    public int LibroId { get; set; }

    /// <summary>
    /// Estado actual del trámite
    /// </summary>
    /// <remarks>
    /// Valores comunes: "Prestado", "Devuelto", "Cerrado". El servidor lo fija por defecto en "Prestado" al crear.
    /// </remarks>
    /// <example>Prestado</example>
    public string Estado { get; set; } = "Prestado";   // Servidor lo fija en POST

    /// <summary>
    /// Fecha de inicio del préstamo
    /// </summary>
    /// <remarks>
    /// Formato obligatorio: "dd-MM-yyyy".
    /// </remarks>
    /// <example>25-10-2025</example>
    public string FechaPrestamo { get; set; } = "";    // dd-MM-yyyy (requerido en POST)

    /// <summary>
    /// Fecha tope para devolver el libro sin multa
    /// </summary>
    /// <remarks>
    /// El servidor calcula esta fecha automáticamente basándose en el rol del usuario.
    /// </remarks>
    /// <example>01-11-2025</example>
    public string FechaLimite { get; set; } = "";      // Servidor lo calcula en POST

    /// <summary>
    /// Fecha real en la que el usuario devolvió el libro
    /// </summary>
    /// <remarks>
    /// Este campo es nulo mientras el préstamo esté activo. Se llena al registrar la devolución.
    /// </remarks>
    /// <example>30-10-2025</example>
    public string? FechaDevolucion { get; set; }       // Solo en devolución
}
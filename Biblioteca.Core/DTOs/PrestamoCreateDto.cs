using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.DTOs;

/// <summary>
/// Representa los datos para crear un Préstamo (DTO) en el sistema
/// </summary>
/// <remarks>
/// Este objeto se utiliza para recibir la información necesaria 
/// al momento de registrar un nuevo préstamo de libro.
/// </remarks>
public class PrestamoCreateDto
{
    /// <summary>
    /// Identificador único del usuario que realiza el préstamo
    /// </summary>
    /// <example>1</example>
    public int UsuarioId { get; set; }

    /// <summary>
    /// Identificador único del libro a prestar
    /// </summary>
    /// <example>5</example>
    public int LibroId { get; set; }

    /// <summary>
    /// Fecha de inicio del préstamo
    /// </summary>
    /// <remarks>
    /// El formato requerido es estrictamente "dd-MM-yyyy".
    /// </remarks>
    /// <example>25-10-2025</example>
    public string FechaPrestamo { get; set; } = null!; // "dd-MM-yyyy"
}
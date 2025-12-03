using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.DTOs;

/// <summary>
/// Respuesta estándar luego de registrar o loguear un usuario.
/// Incluye el token JWT.
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// Id del usuario.
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// Nombre del usuario.
    /// </summary>
    /// <example>Juan Pérez</example>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Email del usuario.
    /// </summary>
    /// <example>juan@example.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Rol asignado al usuario.
    /// </summary>
    /// <example>estudiante</example>
    public string Rol { get; set; } = string.Empty;

    /// <summary>
    /// Indica si el usuario está activo.
    /// </summary>
    /// <example>true</example>
    public bool Activo { get; set; }

    /// <summary>
    /// Token JWT generado para el usuario.
    /// </summary>
    public string Token { get; set; } = string.Empty;
}
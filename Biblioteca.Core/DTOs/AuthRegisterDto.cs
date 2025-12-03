using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.DTOs;
/// <summary>
/// Datos para registrar un nuevo usuario en el sistema.
/// </summary>
public class AuthRegisterDto
{
    /// <summary>
    /// Nombre completo del usuario.
    /// </summary>
    /// <example>Juan Pérez</example>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico del usuario.
    /// </summary>
    /// <example>juan@example.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña en texto plano (solo para entrada).
    /// </summary>
    /// <example>MiPass123</example>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Rol del usuario. Ej: estudiante, staff, admin.
    /// </summary>
    /// <example>estudiante</example>
    public string Rol { get; set; } = "estudiante";
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.DTOs;

/// <summary>
/// Datos para iniciar sesión (login).
/// </summary>
public class AuthLoginDto
{
    /// <summary>
    /// Correo electrónico del usuario.
    /// </summary>
    /// <example>juan@example.com</example>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña en texto plano.
    /// </summary>
    /// <example>MiPass123</example>
    public string Password { get; set; } = string.Empty;
}
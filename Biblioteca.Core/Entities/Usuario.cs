using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Biblioteca.Core.Entities;

/// <summary>
/// Representa un usuario del sistema (estudiante o staff).
/// </summary>
/// <remarks>
/// Esta entidad almacena la información principal del usuario,
/// incluyendo datos personales, rol, estado y credenciales.
/// </remarks>
public class Usuario : BaseEntity
{
    /// <summary>
    /// Nombre completo del usuario.
    /// </summary>
    /// <example>Juan Pérez</example>
    public string Nombre { get; set; } = null!;

    /// <summary>
    /// Correo electrónico del usuario.
    /// </summary>
    /// <example>juan@example.com</example>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Rol del usuario dentro del sistema.
    /// Valores permitidos: estudiante, staff, admin.
    /// </summary>
    /// <example>estudiante</example>
    public string Rol { get; set; } = "estudiante";

    /// <summary>
    /// Indica si el usuario está activo.
    /// </summary>
    /// <example>true</example>
    public bool Activo { get; set; } = true;

    /// <summary>
    /// Hash de la contraseña del usuario.
    /// Nunca se debe almacenar en texto plano.
    /// </summary>
    public string PasswordHash { get; set; } = "";

    /// <summary>
    /// Préstamos asociados al usuario.
    /// </summary>
    public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();

    /// <summary>
    /// Multas asociadas al usuario.
    /// </summary>
    public ICollection<Multa> Multas { get; set; } = new List<Multa>();
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.DTOs
{
    /// <summary>
    /// Representa un Usuario (DTO) en el sistema
    /// </summary>
    /// <remarks>
    /// Este objeto se utiliza para transferir la información pública de los usuarios,
    /// excluyendo datos sensibles como el hash de la contraseña.
    /// </remarks>
    public class UsuarioDto
    {
        /// <summary>
        /// Identificador único del usuario
        /// </summary>
        /// <example>1</example>
        public int Id { get; set; }

        /// <summary>
        /// Nombre completo del usuario
        /// </summary>
        /// <example>Juan Perez</example>
        public string Nombre { get; set; } = "";

        /// <summary>
        /// Correo electrónico institucional o personal
        /// </summary>
        /// <example>juan.perez@estudiantes.edu.bo</example>
        public string Email { get; set; } = "";

        /// <summary>
        /// Rol asignado dentro del sistema
        /// </summary>
        /// <remarks>
        /// Valores permitidos: "estudiante" o "staff".
        /// </remarks>
        /// <example>estudiante</example>
        public string Rol { get; set; } = "estudiante"; // estudiante | staff

        /// <summary>
        /// Indica si el usuario tiene acceso al sistema
        /// </summary>
        /// <example>true</example>
        public bool Activo { get; set; } = true;
    }
}
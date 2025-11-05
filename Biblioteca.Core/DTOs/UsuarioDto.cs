using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Core.DTOs
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Email { get; set; } = "";
        public string Rol { get; set; } = "estudiante"; // estudiante | staff
        public bool Activo { get; set; } = true;
    }
}

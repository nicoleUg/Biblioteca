using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biblioteca.Core.DTOs;

namespace Biblioteca.Core.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Registra un nuevo usuario y devuelve sus datos junto con el token JWT.
        /// </summary>
        Task<AuthResponseDto> RegisterAsync(AuthRegisterDto dto);

        /// <summary>
        /// Valida credenciales y devuelve datos + token JWT.
        /// </summary>
        Task<AuthResponseDto> LoginAsync(AuthLoginDto dto);
    }
}

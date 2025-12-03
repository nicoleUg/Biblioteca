using Microsoft.AspNetCore.Mvc;
using Biblioteca.Core.DTOs;
using Biblioteca.Core.Interfaces;
using Biblioteca.Responses;
using System.Net;

namespace Biblioteca.Controllers;

/// <summary>
/// Endpoints de autenticación (registro y login)
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Registra un nuevo usuario</summary>
    [HttpPost("register")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> Register([FromBody] AuthRegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return StatusCode((int)HttpStatusCode.Created,
            new ApiResponse<AuthResponseDto>(result)
            {
                Messages = new[]
                {
                    new Biblioteca.Core.CustomEntities.Message
                    {
                        Type = "success",
                        Description = "Usuario registrado correctamente"
                    }
                }
            });
    }

    /// <summary>Inicia sesión (login) y devuelve un token JWT</summary>
    [HttpPost("login")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Login([FromBody] AuthLoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(new ApiResponse<AuthResponseDto>(result)
        {
            Messages = new[]
            {
                new Biblioteca.Core.CustomEntities.Message
                {
                    Type = "success",
                    Description = "Inicio de sesión exitoso"
                }
            }
        });
    }
}

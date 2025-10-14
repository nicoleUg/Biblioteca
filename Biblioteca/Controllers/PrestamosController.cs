using AutoMapper;
using Biblioteca.Responses;
using Biblioteca.Core.DTOs;
using Biblioteca.Core.Interfaces;
using Biblioteca.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net;

namespace Biblioteca.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrestamosController : ControllerBase
    {
        private readonly IPrestamoService _service;
        private readonly IMapper _mapper;
        private readonly IValidator<PrestamoCreateDto> _createValidator;

        public PrestamosController(
            IPrestamoService service,
            IMapper mapper,
            IValidator<PrestamoCreateDto> createValidator)
        {
            _service = service;
            _mapper = mapper;
            _createValidator = createValidator;
        }

        [HttpPost]
        public async Task<IActionResult> CrearPrestamo([FromBody] PrestamoCreateDto dto)
        {
            var val = await _createValidator.ValidateAsync(dto);
            if (!val.IsValid) return BadRequest(new { Errors = val.Errors.Select(e => e.ErrorMessage) });

            try
            {
                var f = DateTime.ParseExact(dto.FechaPrestamo, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                var p = await _service.CrearPrestamoAsync(dto.UsuarioId, dto.LibroId, f);

                var response = new ApiResponse<object>(new
                {
                    p.Id,
                    p.UsuarioId,
                    p.LibroId,
                    p.Estado,
                    FechaPrestamo = p.FechaPrestamo.ToString("dd-MM-yyyy"),
                    FechaLimite = p.FechaLimite.ToString("dd-MM-yyyy"),
                    FechaDevolucion = p.FechaDevolucion?.ToString("dd-MM-yyyy")
                });

                return StatusCode((int)HttpStatusCode.Created, response);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost("{id}/devolver")]
        public async Task<IActionResult> Devolver(int id, [FromQuery] string fecha)
        {
            if (!DateTime.TryParseExact(fecha, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var f))
                return BadRequest(new { Errors = new[] { "Formato de fecha inválido (dd-MM-yyyy)" } });

            try
            {
                var p = await _service.DevolverPrestamoAsync(id, f);
                if (p is null) return NotFound();

                var response = new ApiResponse<object>(new
                {
                    p.Id,
                    p.UsuarioId,
                    p.LibroId,
                    p.Estado,
                    FechaPrestamo = p.FechaPrestamo.ToString("dd-MM-yyyy"),
                    FechaLimite = p.FechaLimite.ToString("dd-MM-yyyy"),
                    FechaDevolucion = p.FechaDevolucion?.ToString("dd-MM-yyyy")
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}

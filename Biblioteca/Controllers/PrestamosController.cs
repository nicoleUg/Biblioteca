using AutoMapper;
using Biblioteca.Core.DTOs;
using Biblioteca.Core.Entities;
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
        private readonly IBaseRepository<Prestamo> _repo;
        private readonly IMapper _mapper;
        private readonly IValidator<PrestamoCreateDto> _createValidator;

        public PrestamosController(
            IPrestamoService service,
            IBaseRepository<Prestamo> repo,
            IMapper mapper,
            IValidator<PrestamoCreateDto> createValidator)
        {
            _service = service;
            _repo = repo;
            _mapper = mapper;
            _createValidator = createValidator;
        }

        // GET: api/prestamos
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _repo.GetAll();
            var data = list.Select(p => new
            {
                p.Id,
                p.UsuarioId,
                p.LibroId,
                p.Estado,
                FechaPrestamo = p.FechaPrestamo.ToString("dd-MM-yyyy"),
                FechaLimite = p.FechaLimite.ToString("dd-MM-yyyy"),
                FechaDevolucion = p.FechaDevolucion?.ToString("dd-MM-yyyy")
            });
            return Ok(new ApiResponse<object>(data));
        }

        // GET: api/prestamos/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await _repo.GetById(id);
            if (p is null) return NotFound();
            var data = new
            {
                p.Id,
                p.UsuarioId,
                p.LibroId,
                p.Estado,
                FechaPrestamo = p.FechaPrestamo.ToString("dd-MM-yyyy"),
                FechaLimite = p.FechaLimite.ToString("dd-MM-yyyy"),
                FechaDevolucion = p.FechaDevolucion?.ToString("dd-MM-yyyy")
            };
            return Ok(new ApiResponse<object>(data));
        }

        // POST: api/prestamos
        [HttpPost]
        public async Task<IActionResult> CrearPrestamo([FromBody] PrestamoCreateDto dto)
        {
            var val = await _createValidator.ValidateAsync(dto);
            if (!val.IsValid)
                return BadRequest(new { Errors = val.Errors.Select(e => e.ErrorMessage) });

            try
            {
                var f = DateTime.ParseExact(dto.FechaPrestamo, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                var p = await _service.CrearPrestamoAsync(dto.UsuarioId, dto.LibroId, f);

                var data = new
                {
                    p.Id,
                    p.UsuarioId,
                    p.LibroId,
                    p.Estado,
                    FechaPrestamo = p.FechaPrestamo.ToString("dd-MM-yyyy"),
                    FechaLimite = p.FechaLimite.ToString("dd-MM-yyyy"),
                    FechaDevolucion = p.FechaDevolucion?.ToString("dd-MM-yyyy")
                };

                return StatusCode((int)HttpStatusCode.Created, new ApiResponse<object>(data));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // POST: api/prestamos/{id}/devolver?fecha=dd-MM-yyyy
        [HttpPost("{id:int}/devolver")]
        public async Task<IActionResult> Devolver(int id, [FromQuery] string fecha)
        {
            if (!DateTime.TryParseExact(fecha, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var f))
                return BadRequest(new { Errors = new[] { "Formato de fecha inválido (dd-MM-yyyy)" } });

            try
            {
                var p = await _service.DevolverPrestamoAsync(id, f);
                if (p is null) return NotFound();

                var data = new
                {
                    p.Id,
                    p.UsuarioId,
                    p.LibroId,
                    p.Estado,
                    FechaPrestamo = p.FechaPrestamo.ToString("dd-MM-yyyy"),
                    FechaLimite = p.FechaLimite.ToString("dd-MM-yyyy"),
                    FechaDevolucion = p.FechaDevolucion?.ToString("dd-MM-yyyy")
                };

                return Ok(new ApiResponse<object>(data));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // DELETE: api/prestamos/5  (solo para pruebas)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _repo.GetById(id);
            if (p is null) return NotFound();
            await _repo.Delete(id);
            return NoContent();
        }
    }
}

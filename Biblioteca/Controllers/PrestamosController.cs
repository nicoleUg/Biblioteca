using AutoMapper;
using Biblioteca.Core.CustomEntities;
using Biblioteca.Core.DTOs;
using Biblioteca.Core.Exceptions;
using Biblioteca.Core.Interfaces;
using Biblioteca.Core.QueryFilters;
using Biblioteca.Core.Services;
using Biblioteca.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Biblioteca.Controllers;

/// <summary>
/// Gestión de Préstamos (Dapper para GET, reglas complejas en Service)
/// </summary>
[Produces("application/json")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class PrestamosController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IPrestamoService _service;
    private readonly IMapper _mapper;

    public PrestamosController(IUnitOfWork uow, IPrestamoService service, IMapper mapper)
    {
        _uow = uow;
        _service = service;
        _mapper = mapper;
    }

    // GET — PÚBLICO

    /// <summary>Lista de préstamos (con Dapper + filtros + paginación)</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<PrestamoDto>>))]
    public async Task<IActionResult> Get([FromQuery] PrestamoQueryFilter filters)
    {
        var items = await _uow.PrestamosEx.GetAllDapperAsync(filters.UsuarioId, filters.LibroId, filters.Estado);

        var paged = PagedList<Biblioteca.Core.Entities.Prestamo>.Create(items, filters.PageNumber, filters.PageSize);

        var dto = _mapper.Map<IEnumerable<PrestamoDto>>(paged);

        return Ok(new ApiResponse<IEnumerable<PrestamoDto>>(dto)
        {
            Pagination = new Pagination
            {
                TotalCount = paged.TotalCount,
                PageSize = paged.PageSize,
                CurrentPage = paged.CurrentPage,
                TotalPages = paged.TotalPages,
                HasNextPage = paged.HasNextPage,
                HasPreviousPage = paged.HasPreviousPage
            }
        });
    }

    /// <summary>Obtiene un préstamo por ID (Dapper)</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var e = await _uow.PrestamosEx.GetByIdDapperAsync(id);

        if (e is null) throw new BusinessException("Préstamo no encontrado", 404);

        return Ok(new ApiResponse<PrestamoDto>(_mapper.Map<PrestamoDto>(e)));
    }

    // CREATE — estudiantes y staff
    

    /// <summary>Crea un préstamo aplicando reglas de negocio</summary>
    [HttpPost]
    [Authorize(Roles = "estudiante,staff")]
    public async Task<IActionResult> Create([FromBody] PrestamoCreateDto dto)
    {
        if (!DateTime.TryParseExact(dto.FechaPrestamo, "dd-MM-yyyy",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out var fecha))
        {
            return BadRequest(new ApiResponse<object>(null)
            {
                Messages = new[]
                {
                    new Message { Type = "error", Description = "Formato de fecha inválido. Use dd-MM-yyyy" }
                }
            });
        }

        var p = await _service.CrearPrestamoAsync(dto.UsuarioId, dto.LibroId, fecha);

        return StatusCode((int)HttpStatusCode.Created,
            new ApiResponse<PrestamoDto>(_mapper.Map<PrestamoDto>(p))
            {
                Messages = new[]
                {
                    new Message { Type = "success", Description = "Préstamo registrado correctamente" }
                }
            });
    }

    // DEVOLVER — solo staff
    

    [HttpPost("{id:int}/devolver")]
    [Authorize(Roles = "staff")]
    public async Task<IActionResult> Devolver(int id, [FromQuery] string fecha)
    {
        if (!DateTime.TryParseExact(fecha, "dd-MM-yyyy",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out var f))
        {
            return BadRequest(new ApiResponse<object>(null)
            {
                Messages = new[]
                {
                    new Message { Type = "error", Description = "Formato de fecha inválido. Use dd-MM-yyyy" }
                }
            });
        }

        var p = await _service.DevolverPrestamoAsync(id, f);
        if (p is null) throw new BusinessException("Préstamo no encontrado", 404);

        return Ok(new ApiResponse<PrestamoDto>(_mapper.Map<PrestamoDto>(p))
        {
            Messages = new[]
            {
                new Message { Type = "success", Description = "Préstamo devuelto correctamente" }
            }
        });
    }

    // DELETE — solo staff

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "staff")]
    public async Task<IActionResult> Delete(int id)
    {
        await _uow.BeginTransactionAsync();
        try
        {
            await _uow.Prestamos.Delete(id);
            await _uow.CommitAsync();

            return Ok(new ApiResponse<bool>(true)
            {
                Messages = new[]
                {
                    new Message { Type = "warning", Description = "Préstamo eliminado correctamente" }
                }
            });
        }
        catch
        {
            await _uow.RollbackAsync();
            throw;
        }
    }
}

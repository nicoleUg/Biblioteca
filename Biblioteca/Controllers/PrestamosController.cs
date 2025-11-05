using AutoMapper;
using Biblioteca.Core.CustomEntities;
using Biblioteca.Core.DTOs;
using Biblioteca.Core.Exceptions;
using Biblioteca.Core.Interfaces;
using Biblioteca.Core.QueryFilters;
using Biblioteca.Core.Services;
using Biblioteca.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Biblioteca.Controllers;

[Produces("application/json")]
[ApiController]
[Route("api/[controller]")]
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

    /// <summary>Lista de préstamos (Dapper + filtros + paginación)</summary>
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<PrestamoDto>>))]
    public async Task<IActionResult> Get([FromQuery] PrestamoQueryFilter filters)
    {
        // Usar PrestamosEx para acceder a GetAllDapperAsync
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

    /// <summary>Obtiene un préstamo por Id (Dapper)</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        // Usar PrestamosEx para acceder a GetByIdDapperAsync
        var e = await _uow.PrestamosEx.GetByIdDapperAsync(id);
        if (e is null) throw new BusinessException("Préstamo no encontrado", 404);
        return Ok(new ApiResponse<PrestamoDto>(_mapper.Map<PrestamoDto>(e)));
    }

    /// <summary>Crea un préstamo (reglas de negocio en Service)</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PrestamoCreateDto dto)
    {
        if (!DateTime.TryParseExact(dto.FechaPrestamo, "dd-MM-yyyy",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out var fecha))
        {
            return BadRequest(new ApiResponse<object>(null)
            {
                Messages = new[] { new Message { Type = "error", Description = "Formato de fecha inválido (dd-MM-yyyy)" } }
            });
        }

        var p = await _service.CrearPrestamoAsync(dto.UsuarioId, dto.LibroId, fecha);

        var outDto = _mapper.Map<PrestamoDto>(p);
        return StatusCode((int)HttpStatusCode.Created,
            new ApiResponse<PrestamoDto>(outDto)
            {
                Messages = new[] { new Message { Type = "success", Description = "Préstamo creado" } }
            });
    }

    /// <summary>Registra devolución de préstamo</summary>
    [HttpPost("{id:int}/devolver")]
    public async Task<IActionResult> Devolver(int id, [FromQuery] string fecha)
    {
        if (!DateTime.TryParseExact(fecha, "dd-MM-yyyy",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out var f))
        {
            return BadRequest(new ApiResponse<object>(null)
            {
                Messages = new[] { new Message { Type = "error", Description = "Formato de fecha inválido (dd-MM-yyyy)" } }
            });
        }

        var p = await _service.DevolverPrestamoAsync(id, f);
        if (p is null) throw new BusinessException("Préstamo no encontrado", 404);

        return Ok(new ApiResponse<PrestamoDto>(_mapper.Map<PrestamoDto>(p))
        {
            Messages = new[] { new Message { Type = "success", Description = "Préstamo devuelto" } }
        });
    }

    /// <summary>Elimina un préstamo (solo pruebas)</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _uow.BeginTransactionAsync();
        try
        {
            await _uow.Prestamos.Delete(id);
            await _uow.CommitAsync();

            return Ok(new ApiResponse<bool>(true)
            {
                Messages = new[] { new Message { Type = "warning", Description = "Préstamo eliminado" } }
            });
        }
        catch
        {
            await _uow.RollbackAsync();
            throw;
        }
    }
}
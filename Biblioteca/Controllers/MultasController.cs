using AutoMapper;
using Biblioteca.Core.CustomEntities;
using Biblioteca.Core.DTOs;
using Biblioteca.Core.Exceptions;
using Biblioteca.Core.Interfaces;
using Biblioteca.Core.QueryFilters;
using Biblioteca.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Biblioteca.Controllers;

/// <summary>
/// Gestión de Multas (GET vía Dapper, CRUD vía EF)
/// </summary>
[Produces("application/json")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class MultasController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IMultaService _service;
    private readonly IMapper _mapper;

    public MultasController(IUnitOfWork uow, IMultaService service, IMapper mapper)
    {
        _uow = uow;
        _service = service;
        _mapper = mapper;
    }

    // GET — PÚBLICO

    /// <summary>Lista de multas (Dapper + filtros + paginación)</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get([FromQuery] MultaQueryFilter filters)
    {
        var items = await _uow.MultasEx.GetAllDapperAsync(filters.UsuarioId, filters.Estado);
        var paged = PagedList<Biblioteca.Core.Entities.Multa>.Create(items, filters.PageNumber, filters.PageSize);
        var dto = _mapper.Map<IEnumerable<MultaDto>>(paged);

        return Ok(new ApiResponse<IEnumerable<MultaDto>>(dto)
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

    /// <summary>Obtiene una multa por ID</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var e = await _uow.MultasEx.GetByIdDapperAsync(id);
        if (e is null) throw new BusinessException("Multa no encontrada", 404);
        return Ok(new ApiResponse<MultaDto>(_mapper.Map<MultaDto>(e)));
    }

    
    // CREATE — SOLO STAFF

    /// <summary>Crea una multa (Solo staff)</summary>
    [HttpPost]
    [Authorize(Roles = "staff")]
    public async Task<IActionResult> Create([FromBody] MultaDto dto)
    {
        var entity = _mapper.Map<Biblioteca.Core.Entities.Multa>(dto);

        await _uow.BeginTransactionAsync();
        try
        {
            await _uow.Multas.Add(entity);
            await _uow.CommitAsync();

            dto.Id = entity.Id;

            return StatusCode((int)HttpStatusCode.Created,
                new ApiResponse<MultaDto>(dto)
                {
                    Messages = new[] { new Message { Type = "success", Description = "Multa creada correctamente" } }
                });
        }
        catch
        {
            await _uow.RollbackAsync();
            throw;
        }
    }

    // UPDATE — SOLO STAFF

    /// <summary>Actualiza una multa (Solo staff)</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "staff")]
    public async Task<IActionResult> Update(int id, [FromBody] MultaDto dto)
    {
        var e = await _uow.Multas.GetById(id);
        if (e is null) throw new BusinessException("Multa no encontrada", 404);

        _mapper.Map(dto, e);

        await _uow.BeginTransactionAsync();
        try
        {
            _uow.Multas.Update(e);
            await _uow.CommitAsync();

            return Ok(new ApiResponse<MultaDto>(_mapper.Map<MultaDto>(e))
            {
                Messages = new[] { new Message { Type = "success", Description = "Multa actualizada correctamente" } }
            });
        }
        catch
        {
            await _uow.RollbackAsync();
            throw;
        }
    }


    // ACTIONS — SOLO STAFF

    /// <summary>Pagar multa</summary>
    [HttpPost("{id:int}/pagar")]
    [Authorize(Roles = "staff")]
    public async Task<IActionResult> Pagar(int id)
    {
        var ok = await _service.PagarMultaAsync(id);
        if (!ok) throw new BusinessException("Multa no encontrada", 404);

        return Ok(new ApiResponse<bool>(true)
        {
            Messages = new[] { new Message { Type = "success", Description = "Multa pagada correctamente" } }
        });
    }

    /// <summary>Cancelar multa</summary>
    [HttpPost("{id:int}/cancelar")]
    [Authorize(Roles = "staff")]
    public async Task<IActionResult> Cancelar(int id, [FromQuery] string? motivo = null)
    {
        var ok = await _service.CancelarMultaAsync(id, motivo);
        if (!ok) throw new BusinessException("Multa no encontrada", 404);

        return Ok(new ApiResponse<bool>(true)
        {
            Messages = new[] { new Message { Type = "success", Description = "Multa cancelada correctamente" } }
        });
    }

    // DELETE — SOLO STAFF
   

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "staff")]
    public async Task<IActionResult> Delete(int id)
    {
        await _uow.BeginTransactionAsync();
        try
        {
            await _uow.Multas.Delete(id);
            await _uow.CommitAsync();

            return Ok(new ApiResponse<bool>(true)
            {
                Messages = new[]
                {
                    new Message { Type = "warning", Description = "Multa eliminada correctamente" }
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

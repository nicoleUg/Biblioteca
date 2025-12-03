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
/// Gestión de Libros (GET por Dapper, CRUD por EF, filtros y paginación)
/// </summary>
[Produces("application/json")]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class LibrosController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public LibrosController(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    // ============================================================
    // GET (PÚBLICO) – LISTADO
    // ============================================================

    /// <summary>
    /// Lista de libros filtrada y paginada (GET vía Dapper)
    /// </summary>
    /// <remarks>
    /// Filtros disponibles: titulo, autor, categoria, habilitado.
    /// Paginación: pageNumber, pageSize.
    /// </remarks>
    [HttpGet]
    [AllowAnonymous]  // Public endpoint
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<LibroDto>>))]
    public async Task<IActionResult> Get([FromQuery] LibroQueryFilter filters)
    {
        var items = await _uow.LibrosEx.GetAllDapperAsync(
            filters.Titulo,
            filters.Autor,
            filters.Categoria,
            filters.Habilitado
        );

        var paged = PagedList<Biblioteca.Core.Entities.Libro>.Create(
            items,
            filters.PageNumber,
            filters.PageSize
        );

        var dto = _mapper.Map<IEnumerable<LibroDto>>(paged);

        return Ok(new ApiResponse<IEnumerable<LibroDto>>(dto)
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

    // ============================================================
    // GET POR ID (PÚBLICO)
    // ============================================================

    /// <summary>
    /// Obtiene un libro por su identificador
    /// </summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<LibroDto>))]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var entity = await _uow.LibrosEx.GetByIdDapperAsync(id);

        if (entity is null)
            throw new BusinessException("Libro no encontrado", 404);

        return Ok(new ApiResponse<LibroDto>(_mapper.Map<LibroDto>(entity)));
    }

    // ============================================================
    // CREATE – SOLO STAFF
    // ============================================================

    /// <summary>
    /// Crea un nuevo libro (Solo STAFF)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "staff")]
    [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(ApiResponse<LibroDto>))]
    public async Task<IActionResult> Create([FromBody] LibroDto dto)
    {
        var entity = _mapper.Map<Biblioteca.Core.Entities.Libro>(dto);
        entity.Id = 0; // ignorar si vino en el body

        await _uow.BeginTransactionAsync();
        await _uow.Libros.Add(entity);
        await _uow.CommitAsync();

        dto.Id = entity.Id;

        return StatusCode((int)HttpStatusCode.Created,
            new ApiResponse<LibroDto>(dto)
            {
                Messages = new[]
                {
                    new Message { Type = "success", Description = "Libro creado correctamente" }
                }
            });
    }

    // UPDATE – SOLO STAFF

    /// <summary>
    /// Actualiza los datos de un libro existente (Solo STAFF)
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "staff")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<LibroDto>))]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] LibroDto dto)
    {
        var entity = await _uow.Libros.GetById(id);

        if (entity is null)
            throw new BusinessException("Libro no encontrado", 404);

        entity.Titulo = dto.Titulo;
        entity.Autor = dto.Autor;
        entity.Categoria = dto.Categoria;
        entity.CostoReposicionBs = dto.CostoReposicionBs;
        entity.TotalEjemplares = dto.TotalEjemplares;
        entity.EjemplaresDisponibles = dto.EjemplaresDisponibles;
        entity.CondicionGeneral = dto.CondicionGeneral;
        entity.Habilitado = dto.Habilitado;

        await _uow.BeginTransactionAsync();
        _uow.Libros.Update(entity);
        await _uow.CommitAsync();

        return Ok(new ApiResponse<LibroDto>(_mapper.Map<LibroDto>(entity))
        {
            Messages = new[]
            {
                new Message { Type = "success", Description = "Libro actualizado correctamente" }
            }
        });
    }

    
    // DELETE – SOLO STAFF

    /// <summary>
    /// Elimina un libro del catálogo (Solo STAFF)
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "staff")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<bool>))]
    public async Task<IActionResult> Delete(int id)
    {
        await _uow.BeginTransactionAsync();
        await _uow.Libros.Delete(id);
        await _uow.CommitAsync();

        return Ok(new ApiResponse<bool>(true)
        {
            Messages = new[]
            {
                new Message { Type = "warning", Description = "Libro eliminado correctamente" }
            }
        });
    }
}

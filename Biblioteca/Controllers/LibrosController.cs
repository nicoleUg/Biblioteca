using AutoMapper;
using Biblioteca.Core.CustomEntities;
using Biblioteca.Core.DTOs;
using Biblioteca.Core.Exceptions;
using Biblioteca.Core.Interfaces;
using Biblioteca.Core.QueryFilters;
using Biblioteca.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Biblioteca.Controllers;

/// <summary>
/// Gestión de Libros (GET con Dapper, filtros y paginación)
/// </summary>
[Produces("application/json")]
[ApiController]
[Route("api/[controller]")]
public class LibrosController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public LibrosController(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    /// <summary>Lista de libros filtrada/paginada (GET via Dapper)</summary>
    /// <remarks>Filtros: titulo, autor, categoria, habilitado. Paginación: pageNumber, pageSize.</remarks>
    /// <param name="filters">Filtro de búsqueda y paginación</param>
    /// <response code="200">Retorna la lista de LibroDto</response>
    /// <response code="500">Error interno</response>
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<LibroDto>>))]
    public async Task<IActionResult> Get([FromQuery] LibroQueryFilter filters)
    {
        var items = await _uow.LibrosEx.GetAllDapperAsync(
            filters.Titulo, filters.Autor, filters.Categoria, filters.Habilitado);

        var paged = PagedList<Biblioteca.Core.Entities.Libro>
            .Create(items, filters.PageNumber, filters.PageSize);

        var dto = _mapper.Map<IEnumerable<LibroDto>>(paged);

        var response = new ApiResponse<IEnumerable<LibroDto>>(dto)
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
        };

        return Ok(response);
    }

    /// <summary>Obtiene un libro por ID (Dapper)</summary>
    /// <param name="id">Identificador del libro</param>
    /// <response code="200">Libro encontrado</response>
    /// <response code="404">Libro no encontrado</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<LibroDto>))]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var e = await _uow.LibrosEx.GetByIdDapperAsync(id);
        if (e is null) throw new BusinessException("Libro no encontrado", 404);

        return Ok(new ApiResponse<LibroDto>(_mapper.Map<LibroDto>(e)));
    }

    /// <summary>Crea un nuevo libro (EF)</summary>
    /// <response code="201">Libro creado correctamente</response>
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(ApiResponse<LibroDto>))]
    public async Task<IActionResult> Create([FromBody] LibroDto dto)
    {
        var entity = _mapper.Map<Biblioteca.Core.Entities.Libro>(dto);
        entity.Id = 0; // ignorar Id del body si vino

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

    /// <summary>Actualiza un libro existente (EF)</summary>
    /// <response code="200">Libro actualizado</response>
    /// <response code="404">Libro no encontrado</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<LibroDto>))]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] LibroDto dto)
    {
        var entity = await _uow.Libros.GetById(id);
        if (entity is null) throw new BusinessException("Libro no encontrado", 404);

        // Mapear campos actualizables
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

    /// <summary>Elimina un libro (EF)</summary>
    /// <response code="200">Libro eliminado</response>
    [HttpDelete("{id:int}")]
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

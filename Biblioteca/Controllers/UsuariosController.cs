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

[Produces("application/json")]
[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UsuariosController(IUnitOfWork uow, IMapper mapper)
    { _uow = uow; _mapper = mapper; }

    /// <summary>Lista de usuarios (Dapper + filtros + paginación)</summary>
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<UsuarioDto>>))]
    public async Task<IActionResult> Get([FromQuery] UsuarioQueryFilter filters)
    {
        var items = await _uow.UsuariosEx.GetAllDapperAsync(filters.Nombre, filters.Email, filters.Rol, filters.Activo);
        var paged = PagedList<Biblioteca.Core.Entities.Usuario>.Create(items, filters.PageNumber, filters.PageSize);
        var dto = _mapper.Map<IEnumerable<UsuarioDto>>(paged);

        return Ok(new ApiResponse<IEnumerable<UsuarioDto>>(dto)
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

    /// <summary>Detalle de usuario por id (Dapper)</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var e = await _uow.UsuariosEx.GetByIdDapperAsync(id);
        if (e is null) throw new BusinessException("Usuario no encontrado", 404);
        return Ok(new ApiResponse<UsuarioDto>(_mapper.Map<UsuarioDto>(e)));
    }

    /// <summary>Crea usuario (EF)</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UsuarioDto dto)
    {
        var entity = _mapper.Map<Biblioteca.Core.Entities.Usuario>(dto);
        entity.Id = 0;

        await _uow.BeginTransactionAsync();
        await _uow.Usuarios.Add(entity);
        await _uow.CommitAsync();

        var outDto = _mapper.Map<UsuarioDto>(entity);
        return StatusCode((int)HttpStatusCode.Created, new ApiResponse<UsuarioDto>(outDto)
        { Messages = new[] { new Message { Type = "success", Description = "Usuario creado" } } });
    }

    /// <summary>Actualiza usuario (EF)</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UsuarioDto dto)
    {
        var entity = await _uow.Usuarios.GetById(id);
        if (entity is null) throw new BusinessException("Usuario no encontrado", 404);

        entity.Nombre = dto.Nombre;
        entity.Email = dto.Email;
        entity.Rol = dto.Rol;
        entity.Activo = dto.Activo;

        await _uow.BeginTransactionAsync();
        _uow.Usuarios.Update(entity);
        await _uow.CommitAsync();

        return Ok(new ApiResponse<UsuarioDto>(_mapper.Map<UsuarioDto>(entity))
        { Messages = new[] { new Message { Type = "success", Description = "Usuario actualizado" } } });
    }

    /// <summary>Elimina usuario (EF)</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _uow.BeginTransactionAsync();
        await _uow.Usuarios.Delete(id);
        await _uow.CommitAsync();

        return Ok(new ApiResponse<bool>(true)
        { Messages = new[] { new Message { Type = "warning", Description = "Usuario eliminado" } } });
    }
}

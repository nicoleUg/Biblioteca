using Biblioteca.Core.Entities;
using Biblioteca.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IBaseRepository<Usuario> _repo;

    public UsuariosController(IBaseRepository<Usuario> repo)
    {
        _repo = repo;
    }

    // GET: api/usuarios
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _repo.GetAll();
        return Ok(data.Select(u => new {
            u.Id,
            u.Nombre,
            u.Email,
            u.Rol,
            u.Activo
        }));
    }

    // GET: api/usuarios/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var u = await _repo.GetById(id);
        if (u is null) return NotFound();
        return Ok(new { u.Id, u.Nombre, u.Email, u.Rol, u.Activo });
    }

    // POST: api/usuarios
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Usuario model)
    {
        if (string.IsNullOrWhiteSpace(model.Nombre) || string.IsNullOrWhiteSpace(model.Email))
            return BadRequest(new { message = "Nombre y Email son requeridos" });

        // Rol por defecto si no envían
        if (string.IsNullOrWhiteSpace(model.Rol)) model.Rol = "estudiante";
        if (model.Rol != "estudiante" && model.Rol != "staff")
            return BadRequest(new { message = "Rol debe ser estudiante|staff" });

        model.Activo = model.Activo; // por defecto true en la entidad
        await _repo.Add(model);
        return CreatedAtAction(nameof(GetById), new { id = model.Id }, new { model.Id });
    }

    // PUT: api/usuarios/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Usuario model)
    {
        var u = await _repo.GetById(id);
        if (u is null) return NotFound();

        if (!string.IsNullOrWhiteSpace(model.Nombre)) u.Nombre = model.Nombre;
        if (!string.IsNullOrWhiteSpace(model.Email)) u.Email = model.Email;
        if (!string.IsNullOrWhiteSpace(model.Rol))
        {
            if (model.Rol != "estudiante" && model.Rol != "staff")
                return BadRequest(new { message = "Rol debe ser estudiante|staff" });
            u.Rol = model.Rol;
        }
        u.Activo = model.Activo == u.Activo ? u.Activo : model.Activo;

        await _repo.Update(u);
        return NoContent();
    }

    // DELETE: api/usuarios/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var u = await _repo.GetById(id);
        if (u is null) return NotFound();
        await _repo.Delete(id);
        return NoContent();
    }

    // PUT: api/usuarios/5/activar
    [HttpPut("{id:int}/activar")]
    public async Task<IActionResult> Activar(int id)
    {
        var u = await _repo.GetById(id);
        if (u is null) return NotFound();
        u.Activo = true;
        await _repo.Update(u);
        return NoContent();
    }

    // PUT: api/usuarios/5/desactivar
    [HttpPut("{id:int}/desactivar")]
    public async Task<IActionResult> Desactivar(int id)
    {
        var u = await _repo.GetById(id);
        if (u is null) return NotFound();
        u.Activo = false;
        await _repo.Update(u);
        return NoContent();
    }

    // PUT: api/usuarios/5/rol?rol=staff
    [HttpPut("{id:int}/rol")]
    public async Task<IActionResult> CambiarRol(int id, [FromQuery] string rol)
    {
        if (rol != "estudiante" && rol != "staff")
            return BadRequest(new { message = "Rol debe ser estudiante|staff" });

        var u = await _repo.GetById(id);
        if (u is null) return NotFound();

        u.Rol = rol;
        await _repo.Update(u);
        return NoContent();
    }
}
